using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace KSPMouseInterface
{
    public class GuiPanel
    {
        #region Properties

        public Rect PanelRect { get; set; }
        public Rect PanelRectCollapsed { get; set; }
        public Rect PanelRectExpended { get; set; }

        public GuiPanelState States { get; set; }

        private Texture2D texture = null;
        public Texture2D Texture
        {
            get
            {
                return texture;
            }
            set
            {
                texture = value;
                if (texture != null)
                    Size = new Size(texture.width, texture.height);
            }
        }
        public Texture2D TextureHotSpots { get; set; }

        public bool ParentPanel { get { return Parent == null;} }
        public string Name { get; set; }
        public Alignments Alignment { get; set; }
        public Margin Margin { get; set; }
        public Vector2 Position { get; set; }
        public bool RelativeX { get; set; }
        public bool RelativeY { get; set; }
        public Size Size { get; set; }
        public int OpenCloseBtnHeight { get; set; }
        public string TexturePath { get; set; }
        public string HotSpotTexturePath { get; set; }

        public Color HotSpotColor
        {
            get
            {
                if (States.Color.r == 0 && States.Color.g == 0 && States.Color.b == 0 && States.Color.a == 0)
                {
                    foreach (var entry in Subpanels)
                    {
                        if (entry.Value.States.Color.r != 0 || entry.Value.States.Color.g != 0 || entry.Value.States.Color.b != 0 || entry.Value.States.Color.a != 0)
                            return entry.Value.States.Color;
                    }
                }
                
                return States.Color;
            }
        }

        public GuiPanel Parent { get; set; }
        public Dictionary<string, GuiPanel> Subpanels { get; set; }

        public Dictionary<string, ColorAction> Color2ColorActionMapping { get; set; }
        public Dictionary<Actions, string> Action2ColorMapping { get; set; }

        private bool collapsed = false;
        public bool Collapsed {
            get
            {
                return collapsed;
            }
            set
            {
                collapsed = value;

                if (!collapsed)
                    foreach (var entry in Subpanels)
                        entry.Value.Collapsed = false;

                if (collapsed)
                    PanelRect = PanelRectCollapsed;
                else
                    PanelRect = PanelRectExpended;
            }
        }
        public bool ShowSettingsWindow { get; set; }

        public bool IsEmpty
        {
            get
            {
                return (Name == string.Empty &&
                        Alignment == Alignments.Bottom &&
                        Margin.IsEmpty &&
                        Position.x == 0 && Position.y == 0 &&
                        !RelativeX && !RelativeY &&
                        Size.IsEmpty &&
                        OpenCloseBtnHeight == 0 &&
                        TexturePath == string.Empty &&
                        HotSpotTexturePath == string.Empty &&
                        Parent == null &&
                        Subpanels.Count == 0 &&
                        Color2ColorActionMapping.Count == 0 &&
                        Action2ColorMapping.Count == 0 &&
                        Collapsed == true &&
                        ShowSettingsWindow == false);
            }
        }

        public bool HasValidTexCoords
        {
            get
            {
                return (States.TexCoords.x >= 0 && States.TexCoords.y >= 0 && States.TexCoords.x <= Size.Width && States.TexCoords.y <= Size.Height); 
            }
        }

        public List<string> PropertyAsStrings
        {
            get
            {
                List<string> propertyStrings = new List<string>();
                propertyStrings.Add("=== Interface (" + this.Name + ") ===");
                propertyStrings.Add("Name = " + this.Name.ToString());
                propertyStrings.Add("MainInterface = " + this.ParentPanel.ToString());
                propertyStrings.Add("Alignment = " + this.Alignment.ToString());
                propertyStrings.Add("Collapsed = " + this.Collapsed.ToString());
                propertyStrings.Add("TexturePath = " + this.TexturePath.ToString());
                propertyStrings.Add("HotSpotTexturePath = " + this.HotSpotTexturePath.ToString());
                propertyStrings.Add("Margin = " + this.Margin.ToString());
                propertyStrings.Add("OpenCloseBtnHeight = " + this.OpenCloseBtnHeight.ToString());
                propertyStrings.Add("HasParent = " + ((this.Parent != null) ? true.ToString() : false.ToString()));
                propertyStrings.Add("Position = " + this.Position.ToString());
                propertyStrings.Add("RelativeX = " + this.RelativeX.ToString());
                propertyStrings.Add("RelativeY = " + this.RelativeY.ToString());
                propertyStrings.Add("ShowSettingsWindow = " + this.ShowSettingsWindow.ToString());
                propertyStrings.Add("Size = " + this.Size.ToString());

                propertyStrings.Add("InterfacePos = " + this.PanelRect.ToString());
                propertyStrings.Add("InterfacePosCollapsed = " + this.PanelRectCollapsed.ToString());
                propertyStrings.Add("InterfacePosExpended = " + this.PanelRectExpended.ToString());

                propertyStrings.Add((Texture == null) ? "TextureMC_Interface = null" : "TextureMC_Interface = OK");
                propertyStrings.Add((TextureHotSpots == null) ? "TextureMC_Interface_HotSpots = null" : "TextureMC_Interface_HotSpots = OK");

                int i = 0;
                propertyStrings.Add("== ColorActionMapping (" + Color2ColorActionMapping.Count + ") ==");
                foreach (var entry in Color2ColorActionMapping)
                    propertyStrings.Add(string.Format("ColorAction {0} = {1}", i++, entry.ToString())); //string.Format("ColorAction {0} = Name: {1}, Action: {2}, Color: {3}", i++, entry.Name, entry.Action, entry.Color));

                //propertyStrings.Add("States: " + this.States.ToString());

                foreach (var sub in Subpanels)
                {
                    propertyStrings.Add("=== Subinterface (" + sub.Value.Name + ") ===");
                    propertyStrings.AddRange(sub.Value.PropertyAsStrings);
                }

                return propertyStrings;
            }
        }

        public List<ButtonGraphic> ButtonsToDraw { get; set; }

        #endregion


        public GuiPanel()
        {
            PanelRect = new Rect();
            PanelRectCollapsed = new Rect();
            PanelRectExpended = new Rect();

            States = new GuiPanelState();

            Texture = null;
            TextureHotSpots = null;

            Name = string.Empty;
            Alignment = Alignments.Bottom;
            Margin = new Margin();
            Position = new Vector2();
            RelativeX = false;
            RelativeY = false;
            Size = new Size();
            OpenCloseBtnHeight = 0;
            TexturePath = string.Empty;
            HotSpotTexturePath = string.Empty;
            Parent = null;
            Subpanels = new Dictionary<string, GuiPanel>();
            Color2ColorActionMapping = new Dictionary<string, ColorAction>();
            Action2ColorMapping = new Dictionary<Actions, string>();
            Collapsed = true;
            ShowSettingsWindow = false;
            ButtonsToDraw = new List<ButtonGraphic>();
        }


        public void CalculateMainPanelPos()
        {
            Rect posCol = new Rect(0, 0, Size.Width, Size.Height);
            Rect posExp = new Rect(0, 0, Size.Width, Size.Height);
            //if (Alignment == MCAlignment.Top)
            //{ }
            //else if (Alignment == MCAlignment.Bottom)
            //{
                #region Calc bottom alignment

                if (RelativeX)
                    posCol.x = Screen.width * (Position.x / 100);
                else
                    posCol.x = Position.x;

                posCol.x += Margin.Left;
                posCol.x -= Margin.Right;

                posExp.x = posCol.x;

                // Collapsed
                if (RelativeY)
                    posCol.y = Screen.height * (Position.y / 100) - OpenCloseBtnHeight;
                else
                    posCol.y = Position.y - OpenCloseBtnHeight;

                posCol.y += Margin.Top;
                posCol.y -= Margin.Bottom;

                // Expanded
                if (RelativeY)
                    posExp.y = Screen.height * (Position.y / 100) - Size.Height;
                else
                    posExp.y = Position.y - Size.Height;

                posExp.y += Margin.Top;
                posExp.y -= Margin.Bottom;

                PanelRectCollapsed = posCol;
                PanelRectExpended = posExp;

                #endregion
            //}
            //else if (Alignment == MCAlignment.Left)
            //{ }
            //else if (Alignment == MCAlignment.Right)
            //{ }

            Collapsed = true;
        }

        public void CalculateChildPanelPos()
        {
            PanelRectCollapsed = new Rect(Parent.PanelRectExpended.x + Position.x, Parent.PanelRectExpended.y + Position.y, Size.Width, Size.Height);
            PanelRectExpended = new Rect(Parent.PanelRectExpended.x + Position.x, Parent.PanelRectExpended.y + Position.y, Size.Width, Size.Height);

            if (Alignment == Alignments.Top)
            {
                PanelRectCollapsed = new Rect(PanelRectExpended.x, PanelRectExpended.y, PanelRectCollapsed.width, PanelRectCollapsed.height);
                PanelRectExpended = new Rect(PanelRectExpended.x, PanelRectExpended.y - Size.Height, PanelRectExpended.width, PanelRectExpended.height);
            }
            else if (Alignment == Alignments.Bottom)
            {
                PanelRectCollapsed = new Rect(PanelRectExpended.x, PanelRectExpended.y, PanelRectCollapsed.width, PanelRectCollapsed.height);
                PanelRectExpended = new Rect(PanelRectExpended.x, PanelRectExpended.y + Size.Height, PanelRectExpended.width, PanelRectExpended.height);
            }
            else if (Alignment == Alignments.Left)
            {
                PanelRectCollapsed = new Rect(PanelRectExpended.x, PanelRectExpended.y, PanelRectCollapsed.width, PanelRectCollapsed.height);
                PanelRectExpended = new Rect(PanelRectExpended.x - Size.Width, PanelRectExpended.y, PanelRectExpended.width, PanelRectExpended.height);
            }
            else if (Alignment == Alignments.Right)
            {
                PanelRectCollapsed = new Rect(PanelRectExpended.x, PanelRectExpended.y, PanelRectCollapsed.width, PanelRectCollapsed.height);
                PanelRectExpended = new Rect(PanelRectExpended.x + Size.Width, PanelRectExpended.y, PanelRectExpended.width, PanelRectExpended.height);
            }

            Collapsed = true;
        }
        
        public void UpdateTexCoords(Vector2 mousePosition)
        {
            int x = (int)mousePosition.x - (int)PanelRect.x;
            int y = Size.Height - ((int)mousePosition.y - (int)PanelRect.y);
            States.TexCoords = new Vector2(x, y);

            Color color = new Color(0, 0, 0, 0);
            if (HasValidTexCoords && (ParentPanel || (!ParentPanel && !Collapsed)))
            {
                color = TextureHotSpots.GetPixel((int)States.TexCoords.x,
                                                              (int)States.TexCoords.y);
                color = new Color(color.r * 255, color.g * 255, color.b * 255, color.a * 255);
            }
            States.Color = color;
        }


        public static Dictionary<string, GuiPanel> CreateFromXml(string path)
        {
            var guiPanels = new Dictionary<string, GuiPanel>();

            var doc = new XmlDocument();
            doc.Load(path.Replace("/", "\\"));

            var interfaceNodes = doc.GetElementsByTagName(Consts.cInterface);
            foreach (XmlNode node in interfaceNodes)
            {
                var guiPanel = GuiPanel.CreateFrom(node);
                if (guiPanel == null || guiPanel.IsEmpty)
                    continue;

                guiPanels.Add(guiPanel.Name, guiPanel);
            }

            return guiPanels;
        }

        private static GuiPanel CreateFrom(XmlNode node)
        {
            var newPanel = new GuiPanel();

            var posX = string.Empty;
            var posY = string.Empty;
            //var size = new Size();

            foreach (XmlAttribute att in node.Attributes)
            {
                if (att.Name == Consts.cName)
                    newPanel.Name = att.Value.ToString();

                else if (att.Name == Consts.cAlignment)
                    newPanel.Alignment = Parse.AlignmentFrom(att.Value.ToString().Trim());

                else if (att.Name == Consts.cMargin)
                    newPanel.Margin = Parse.MarginFrom(att.Value.ToString().Trim());

                else if (att.Name == Consts.cX)
                    posX = att.Value;

                else if (att.Name == Consts.cY)
                    posY = att.Value;

                else if (att.Name == Consts.cOpenCloseButtonHeight)
                    newPanel.OpenCloseBtnHeight = Parse.IntFrom(att.Value);

                //else if (att.Name == Consts.cWidth)
                //    size.Width = Parse.IntFrom(att.Value);

                //else if (att.Name == Consts.cHeight)
                //    size.Height = Parse.IntFrom(att.Value);

                else if (att.Name == Consts.cTexturePath)
                    newPanel.TexturePath = att.Value.ToString();

                else if (att.Name == Consts.cHotSpotTexturePath)
                    newPanel.HotSpotTexturePath = att.Value.ToString();
            }

            if (posX.Contains("%"))
                newPanel.RelativeX = true;
            if (posY.Contains("%"))
                newPanel.RelativeY = true;

            var pos = new Vector2();
            if (!string.IsNullOrEmpty(posX))
                pos.x = Parse.IntFrom(posX);
            if (!string.IsNullOrEmpty(posY))
                pos.y = Parse.IntFrom(posY);

            newPanel.Position = pos;
            //newInterface.Size = size;

            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.Name == Consts.cSubInterface)
                    CreateAndAddSubpanelFrom(childNode, newPanel);

                else if (childNode.Name == Consts.cColorActionMapping)
                    CreateAndAddColorActioMappingFrom(childNode, newPanel);
            }

            return newPanel;
        }

        private static void CreateAndAddSubpanelFrom(XmlNode node, GuiPanel parentGuiInterface)
        {
            var subpanel = CreateFrom(node);
            if (subpanel == null || !subpanel.IsEmpty)
            {
                subpanel.Parent = parentGuiInterface;
                parentGuiInterface.Subpanels.Add(subpanel.Name, subpanel);
            }
        }

        private static void CreateAndAddColorActioMappingFrom(XmlNode node, GuiPanel parentGuiPanel)
        {
            foreach (XmlNode colorActionNode in node.ChildNodes)
            {
                if (colorActionNode.Name == Consts.cColorAction)
                {
                    var colorAction = ColorAction.CreateFrom(colorActionNode);
                    try
                    {
                        if (colorAction == null || !colorAction.IsEmpty)
                        {
                            colorAction.Interface = parentGuiPanel;
                            parentGuiPanel.Color2ColorActionMapping.Add(colorAction.Color.ToString(), colorAction);
                            if (!parentGuiPanel.Action2ColorMapping.ContainsKey(colorAction.Action))
                                parentGuiPanel.Action2ColorMapping.Add(colorAction.Action, colorAction.Color.ToString());
                        }
                    }
                    catch
                    {
                        //print(string.Format("Can't add ColorAction! Color ({0}) already exists.", colorAction.Color));
                    }
                }
            }
        }
    }
}
