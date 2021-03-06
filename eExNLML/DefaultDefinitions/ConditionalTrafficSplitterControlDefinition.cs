﻿// This source file is part of the eEx Network Library Management Layer (NLML)
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
using eExNetworkLibrary.TrafficSplitting;
using eExNLML.Extensibility;
using eExNLML.IO;
using eExNLML.IO.HandlerConfigurationLoaders;
using eExNLML.IO.HandlerConfigurationWriters;
using eExNetworkLibrary;
using eExNLML.DefaultControllers;

namespace eExNLML.DefaultDefinitions
{
    public class ConditionalTrafficSplitterControlDefinition : HandlerDefinition
    {
        public ConditionalTrafficSplitterControlDefinition()
        {
            Name = "Conditional Traffic Splitter";
            Description = "This traffic handler is capable of seperating traffic streams by given rules. If no rule matches, the frame is forwarded to handler A by default.";
            Author = "Emanuel Jöbstl";
            WebLink = "http://www.eex-dev.net";
            PluginType = PluginTypes.TrafficHandler;
            PluginKey = "eex_conditional_splitter";
        }

        public override IHandlerController Create(IEnvironment env)
        {
            return new ConditionalTrafficSplitterController(this, env);
        }
    }
}
