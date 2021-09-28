﻿using k8config.DataModels;
using k8s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace k8config
{
    partial class Program
    {
        static Toplevel topLevelWindowObject = Application.Top;
        static ColorScheme colorNormal = new ColorScheme();
        static ColorScheme colorSelector = new ColorScheme();
        static StatusBar statusBar = new StatusBar();
        static void SetupTopLevelView()
        {

            topLevelWindowObject.Clear();
            topLevelWindowObject.TabStop = true;
            topLevelWindowObject.ColorScheme.Normal = new Terminal.Gui.Attribute(Color.Black, Color.White);
            topLevelWindowObject.Add(statusBar);

            colorNormal = new ColorScheme()
            {
                Normal = new Terminal.Gui.Attribute(Color.Black, Color.White),
                Focus = new Terminal.Gui.Attribute(Color.White, Color.Blue),
                HotNormal = new Terminal.Gui.Attribute(Color.Red, Color.White),
                HotFocus = new Terminal.Gui.Attribute(Color.Blue, Color.White),
                Disabled = new Terminal.Gui.Attribute(Color.DarkGray, Color.Black)
            };
            colorSelector = new ColorScheme()
            {
                Normal = new Terminal.Gui.Attribute(Color.Black, Color.White),
                Focus = new Terminal.Gui.Attribute(Color.White, Color.Red),
                HotNormal = new Terminal.Gui.Attribute(Color.White, Color.Red),
                HotFocus = new Terminal.Gui.Attribute(Color.White, Color.Red),
                Disabled = new Terminal.Gui.Attribute(Color.DarkGray, Color.Black)
            };
            //Fire top level drawEvents
            topLevelWindowObject.DrawContent += (e) =>
            {

                if (GlobalVariables.displayMode == 0)
                {
 
                    availableKindsWindow.Width = Convert.ToInt16(topLevelWindowObject.Bounds.Width * 0.30);
                    availableKindsWindow.Height = Convert.ToInt16(topLevelWindowObject.Bounds.Height * 0.70);
                    definedYAMLWindow.X = availableKindsWindow.Bounds.Right;
                    definedYAMLWindow.Height = availableKindsWindow.Bounds.Height;
                    commandWindow.Y = topLevelWindowObject.Bounds.Height - 4;
                    descriptionWindow.X = 0;
                    descriptionWindow.Y = availableKindsWindow.Bounds.Bottom;
                    descriptionWindow.Height = topLevelWindowObject.Bounds.Height-definedYAMLWindow.Height - commandWindow.Height-1;
                }
                else if (GlobalVariables.displayMode == 1)
                {
                    availableContextsWindow.Width = Convert.ToInt16(topLevelWindowObject.Bounds.Width * 0.20);
                    contextDetailTabs.X = availableContextsWindow.Bounds.Right;
                    contextDetailTabs.Width = Dim.Fill();
                }
            };
        }
    }
}
