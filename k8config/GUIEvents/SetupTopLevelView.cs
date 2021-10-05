using k8config.DataModels;
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
        public static Toplevel topLevelWindowObject = Application.Top;
        public static ColorScheme colorNormal = new ColorScheme();
        public static ColorScheme colorSelector = new ColorScheme();
        public static StatusBar statusBar = new StatusBar();
        public static void SetupTopLevelView()
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
                    YAMLModelControls.descriptionView.Width = Dim.Fill();
                    YAMLModelControls.descriptionView.Height = Dim.Fill();
                    YAMLModelControls.availableKindsWindow.Width = Convert.ToInt16(topLevelWindowObject.Bounds.Width * 0.30);
                    YAMLModelControls.availableKindsWindow.Height = Convert.ToInt16(topLevelWindowObject.Bounds.Height * 0.70);
                    YAMLModelControls.definedYAMLWindow.X = YAMLModelControls.availableKindsWindow.Bounds.Right;
                    YAMLModelControls.definedYAMLWindow.Height = YAMLModelControls.availableKindsWindow.Bounds.Height;
                    YAMLModelControls.commandWindow.Y = topLevelWindowObject.Bounds.Height - 4;
                    YAMLModelControls.descriptionWindow.X = 0;
                    YAMLModelControls.descriptionWindow.Y = YAMLModelControls.availableKindsWindow.Bounds.Bottom;
                    YAMLModelControls.descriptionWindow.Height = topLevelWindowObject.Bounds.Height - YAMLModelControls.definedYAMLWindow.Height - YAMLModelControls.commandWindow.Height-1;
                }
                else if (GlobalVariables.displayMode == 1)
                {
                    RealtimeModeControls.availableContextsWindow.Width = Convert.ToInt16(topLevelWindowObject.Bounds.Width * 0.20);
                    RealtimeModeControls.contextDetailTabs.X = RealtimeModeControls.availableContextsWindow.Bounds.Right;
                    RealtimeModeControls.contextDetailTabs.Width = Dim.Fill();
                }
                YAMLModelControls.k8configVersion.X = topLevelWindowObject.Bounds.Width - YAMLModelControls.k8configVersion.Text.Length - 2;
                YAMLModelControls.k8configVersion.Y = topLevelWindowObject.Bounds.Height - 1;
                YAMLModelControls.k8configVersion.ColorScheme = new ColorScheme() { Normal = new Terminal.Gui.Attribute(Color.White, Color.DarkGray) };
                topLevelWindowObject.BringSubviewToFront(YAMLModelControls.k8configVersion);
            };
        }
    }
}
