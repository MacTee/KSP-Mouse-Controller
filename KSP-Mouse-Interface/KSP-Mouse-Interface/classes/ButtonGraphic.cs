using System.Xml;
using UnityEngine;

namespace KSPMouseInterface
{
    // Wraps a Button graphic for a certain ButtonAction.
    public class ButtonGraphic
    {
        #region Member

        private Rect mPosition = new Rect();
        private Rect mTexturePosition = new Rect();
        private Texture2D mTexture = null;

        #endregion

        #region Properties

        // The parent ColorAction.
        public ColorAction ColorAction { get; set; }

        // Action of the Button (Click, hover)
        public ButtonActions Action { get; set; }

        // path to the texture.
        public string TexturePath { get; set; }

        // Top of the OnScreen position rect (in pixel).
        public int X { get; set; }
        // Left of the OnScreen position rect (in pixel).
        public int Y { get; set; }
        // Width of the OnScreen position rect (in pixel).
        public int Width
        {
            get
            {
                if (Texture == null)
                    return 0;
                else
                    return Texture.width;
            }
        }
        // Height of the OnScreen position rect (in pixel).
        public int Height
        {
            get
            {
                if (Texture == null)
                    return 0;
                else
                    return Texture.height;
            }
        }

        // OnScreen render position rect (in pixel).
        public Rect Position
        {
            get
            {
                if (mPosition.x == 0 && mPosition.x == 0 && mPosition.y == 0 && mPosition.width == 0 && mPosition.height == 0)
                    CalcPosition();

                return mPosition;
            }
        }

        // Top of the rect that envelop the texture pert to render (in pixel).
        public int TextureX { get; set; }
        // Left of the rect that envelop the texture pert to render (in pixel).
        public int TextureY { get; set; }
        // Width of the rect that envelop the texture pert to render (in pixel).
        public int TextureWidth { get; set; }
        // Height of the rect that envelop the texture pert to render (in pixel).
        public int TextureHeight { get; set; }

        // The rect that envelop the texture part to render (in normalized format).
        public Rect TexturePosition
        {
            get
            {
                if (HasTextureCoords && mTexturePosition.x == 0 && mTexturePosition.x == 0 && mTexturePosition.y == 0 && mTexturePosition.width == 0 && mTexturePosition.height == 0)
                    CalcPosition();

                return mTexturePosition;
            }
        }

        // Flag to determine whether there are valid texture coords or not.
        public bool HasTextureCoords
        {
            get
            {
                return (TextureX != 0 || TextureY != 0 || TextureWidth != 0 || TextureHeight != 0);
            }
        }

        // The texture to render from.
        public Texture2D Texture
        {
            get
            {
                return mTexture;
            }
            set
            {
                mTexture = value;

                if (mTexture != null)
                    CalcPosition();
            }
        }

        // Flag to determine whether this instance is empty or not.
        public bool IsEmpty
        {
            get
            {
                return (ColorAction == null &&
                        Action == ButtonActions.None &&
                        TexturePath == string.Empty &&
                        X == 0 &&
                        Y == 0 &&
                        TextureX == 0 &&
                        TextureY == 0 &&
                        TextureWidth == 0 &&
                        TextureHeight == 0 &&
                        Texture == null);
            }
        }

        #endregion


        public ButtonGraphic()
        {
            ColorAction = null;
            Action = ButtonActions.None;
            TexturePath = string.Empty;
            X = 0;
            Y = 0;
            TextureX = 0;
            TextureY = 0;
            TextureWidth = 0;
            TextureHeight = 0;
            Texture = null;
        }

        public static ButtonGraphic CreateFrom(XmlNode node)
        {
            var button = new ButtonGraphic();
            foreach (XmlAttribute att in node.Attributes)
            {
                if (att.Name == Consts.cAction)
                    button.Action = Parse.ButtonActionFrom(att.Value.ToString().Trim());

                else if (att.Name == Consts.cTexturePath)
                    button.TexturePath = att.Value.ToString();

                else if (att.Name == Consts.cX)
                    button.X = Parse.IntFrom(att.Value.ToString().Trim());

                else if (att.Name == Consts.cY)
                    button.Y = Parse.IntFrom(att.Value.ToString().Trim());

                //else if (att.Name == Consts.cWidth)
                //    button.Width = Parse.IntFrom(att.Value.ToString().Trim());

                //else if (att.Name == Consts.cHeight)
                //    button.Height = Parse.IntFrom(att.Value.ToString().Trim());

                else if (att.Name == Consts.cTextureX)
                    button.TextureX = Parse.IntFrom(att.Value.ToString().Trim());

                else if (att.Name == Consts.cTextureY)
                    button.TextureY = Parse.IntFrom(att.Value.ToString().Trim());

                else if (att.Name == Consts.cTextureWidth)
                    button.TextureWidth = Parse.IntFrom(att.Value.ToString().Trim());

                else if (att.Name == Consts.cTextureHeight)
                    button.TextureHeight = Parse.IntFrom(att.Value.ToString().Trim());
            }

            return button;
        }


        public override string ToString()
        {
            return string.Format("(Action = {0}, TexturePath = {1}, Position = {2}, TexturePosition = {3}, Texture = {4})",
                                 Action, TexturePath, Position, TexturePosition, (Texture == null) ? "null" : "OK");
        }


        // Calculates the on screen rect and the texture area rect.
        private void CalcPosition()
        {
            if (HasTextureCoords)
            {
                mPosition = new Rect(X, Y, TextureWidth, TextureHeight);
                if (Texture.width > 0 && Texture.height > 0)
                    mTexturePosition = new Rect((float)TextureX / (float)Texture.width, (float)TextureY / (float)Texture.height,
                                                (float)TextureWidth / (float)Texture.width, (float)TextureHeight / (float)Texture.height);
            }
            else
                mPosition = new Rect(X, Y, Width, Height);
        }
    }
}