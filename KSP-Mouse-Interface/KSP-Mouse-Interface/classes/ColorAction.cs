using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace KSPMouseInterface
{
    // Maps a color to an action.
    public class ColorAction
    {
        #region Properties

        // The parent Interface
        public GuiPanel Interface { get; set; }

        // Name of the interface the action is for.
        public string Name { get; set; }

        // The action that should be executed when this button is clicked.
        public Actions Action { get; set; }

        // The color of the Button on the hot spot graphic.
        public Color Color { get; set; }

        public Vector2 Center { get; set; }

        public float Radius { get; set; }

        // List of button states
        public Dictionary<ButtonActions, ButtonGraphic> ButtonGraphics { get; set; }

        // Flag to determine whether this instance is empty or not.
        public bool IsEmpty
        {
            get
            {
                return (Interface == null && Name == string.Empty && Action == Actions.None && Color.r == 0 && Color.g == 0 && Color.b == 0 && Color.a == 0 && ButtonGraphics.Count == 0);
            }
        }

        #endregion


        public ColorAction()
        {
            Interface = null;
            Name = string.Empty;
            Action = Actions.None;
            Color = new Color();

            ButtonGraphics = new Dictionary<ButtonActions, ButtonGraphic>();
        }

        public static ColorAction CreateFrom(XmlNode node)
        {
            var colorAction = new ColorAction();
            foreach (XmlAttribute att in node.Attributes)
            {
                if (att.Name == Consts.cName)
                    colorAction.Name = att.Value.ToString();

                else if (att.Name == Consts.cColor)
                    colorAction.Color = Parse.ColorFrom(att.Value.ToString().Trim());

                else if (att.Name == Consts.cAction)
                    colorAction.Action = Parse.ActionFrom(att.Value.ToString().Trim());

                else if (att.Name == Consts.cCenter)
                    colorAction.Center = Parse.Position2dFrom(att.Value.ToString().Trim());

                else if (att.Name == Consts.cRadius)
                    colorAction.Radius = Parse.FloatFrom(att.Value.ToString().Trim());
            }

            foreach (XmlNode buttonNode in node.ChildNodes)
            {
                if (buttonNode.Name == Consts.cButtonGraphic)
                {
                    var button = ButtonGraphic.CreateFrom(buttonNode);
                    if (!button.IsEmpty)
                    {
                        button.ColorAction = colorAction;
                        colorAction.ButtonGraphics.Add(button.Action, button);
                    }
                }
            }

            return colorAction;
        }


        public override string ToString()
        {
            int i = 0;
            string buttons = string.Empty;
            foreach (var entry in ButtonGraphics)
                buttons += ", ButtonGraphics (" + i++ + ") = " + entry.Value.ToString();
            if (ButtonGraphics.Count == 0)
                buttons = ", ButtonGraphics = 0";
            return string.Format("(Name = {0}, Action = {1}, Color = {2}{3})", Name, Action, Color, buttons);
        }
    }
}