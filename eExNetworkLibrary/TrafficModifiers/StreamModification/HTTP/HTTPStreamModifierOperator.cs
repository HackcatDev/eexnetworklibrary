﻿// This source file is part of the eEx Network Library
//
// Author: 	    Emanuel Jöbstl <emi@eex-dev.net>
// Weblink: 	http://network.eex-dev.net
//		        http://eex.codeplex.com
//
// Licensed under the GNU Library General Public License (LGPL) 
//
// (c) eex-dev 2007-2011

using System;
using System.Collections.Generic;
using System.Text;
using eExNetworkLibrary.Sockets;
using eExNetworkLibrary.Utilities;
using eExNetworkLibrary.HTTP;

namespace eExNetworkLibrary.TrafficModifiers.StreamModification.HTTP
{
    public class HTTPStreamModifierOperator : HTTPStreamOperator
    {
        private HTTPStreamModifierAction[] arStreamActions;

        public HTTPStreamModifierOperator(NetworkStream sAlice, NetworkStream sBob, HTTPStreamModifierAction[] arStreamActions)
            : base(sAlice, sBob)
        {
            this.arStreamActions = arStreamActions;
        }

        protected override eExNetworkLibrary.HTTP.HTTPMessage ModifyRequest(eExNetworkLibrary.HTTP.HTTPRequest httpRequest)
        {
            return ModifyMessage(httpRequest);
        }

        protected override eExNetworkLibrary.HTTP.HTTPMessage ModifyResponse(eExNetworkLibrary.HTTP.HTTPResponse HTTPResponse)
        {
            return ModifyMessage(HTTPResponse);
        }

        private HTTPMessage ModifyMessage(eExNetworkLibrary.HTTP.HTTPMessage httpMessage)
        {
            byte[] bPayload = httpMessage.Payload;

            string strTransferEncoding;

            if (httpMessage.Headers.Contains("Transfer-Encoding"))
            {
                strTransferEncoding = httpMessage.Headers["Transfer-Encoding"][0].Value.ToLower();
                if (strTransferEncoding == "chunked")
                {
                    bPayload = CompressionHelper.DecompressChunked(bPayload);
                    httpMessage.Headers.Remove("Transfer-Encoding");
                    httpMessage.Headers.Add(new HTTPHeader("Content-Length", bPayload.Length.ToString()));
                }
            }

            string strContentEncoding = null;

            if (httpMessage.Headers.Contains("Content-Length"))
            {
                if (httpMessage.Headers.Contains("Content-Encoding"))
                {
                    strContentEncoding = httpMessage.Headers["Content-Encoding"][0].Value.ToLower();
                    if (strContentEncoding == "gzip" || strContentEncoding == "x-gzip")
                    {
                        bPayload = CompressionHelper.DecompressGZip(bPayload);
                    }
                    else if (strContentEncoding == "deflate")
                    {
                        bPayload = CompressionHelper.DecompressDeflate(bPayload);
                    }
                    else if (strContentEncoding == "chunked")
                    {
                        bPayload = CompressionHelper.DecompressChunked(bPayload);
                        strContentEncoding = "none";
                    }
                }

            }

            httpMessage.Payload = bPayload;

            foreach (HTTPStreamModifierAction htAction in arStreamActions)
            {
                if (htAction.IsMatch(httpMessage))
                {
                    httpMessage = htAction.ApplyAction(httpMessage);
                }
            }


            bPayload = httpMessage.Payload;


            if (strContentEncoding != null)
            {
                if (strContentEncoding == "gzip" || strContentEncoding == "x-gzip")
                {
                    bPayload = CompressionHelper.CompressGZip(bPayload);
                }
                else if (strContentEncoding == "deflate")
                {
                    bPayload = CompressionHelper.CompressDeflate(bPayload);
                }
            }

            if (bPayload.Length != 0)
            {
                if (httpMessage.Headers.Contains("Content-Length"))
                {
                    httpMessage.Headers["Content-Length"][0].Value = bPayload.Length.ToString();
                }
                else
                {
                    httpMessage.Headers.Add(new HTTPHeader("Content-Length", bPayload.Length.ToString()));
                }
                httpMessage.Payload = bPayload;
            }

            return httpMessage;
        }

        public override string Description
        {
            get { return "Edit HTTP Message"; }
        }
    }
}
