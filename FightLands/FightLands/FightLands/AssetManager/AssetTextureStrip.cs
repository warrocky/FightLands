using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FightLands
{
    class AssetTextureStrip
    {
        public readonly String name;

        Texture2D _texture;
        public Texture2D texture
        {
            set { setContent(value); }
            get { return _texture; }
        }

        Vector2 _center;
        public Vector2 center
        {
            get { return _center; }
        }


        Point _size;
        public int width
        {
            get { return _size.X; }
        }
        public int height
        {
            get { return _size.Y; }
        }

        public Point size
        {
            get { return _size; }
        }


        StripOrientation _orientation;
        public StripOrientation orietantion
        {
            get { return _orientation; }
        }
        
        int _length;
        public int length
        {
            get { return _length; }
        }

        Rectangle[] rectangleFrames;

        public enum StripOrientation{ LeftToRight, TopToBottom, LeftToRightAndTopToBottom, TopToBottomAndLeftToRight }

        public AssetTextureStrip(String name, Texture2D content, StripOrientation orientation, Point frameSize)
        {
            this.name = name;
            this._texture = content;
            this._orientation = orientation;
            _size = frameSize;

            UpdateRectangleFrames();

            _length = rectangleFrames.Length;
        }

        public void setContent(Texture2D newContent)
        {
            _texture = newContent;

            UpdateRectangleFrames();

            _length = rectangleFrames.Length;
        }
        public void setContent(Texture2D newContent, Point newFrameSize, StripOrientation newStripOrientation)
        {
            _size = newFrameSize;
            _texture = newContent;
            _orientation = newStripOrientation;

            _center = new Vector2(_size.X, _size.Y) / 2f;

            UpdateRectangleFrames();

            _length = rectangleFrames.Length;
        }

        private void UpdateRectangleFrames()
        {
            if (_texture.Width == 0 || _texture.Height == 0)
                throw new Exception("Invalid texture format");

            if (_texture.Width % size.X != 0)
                throw new Exception("Framesize does not complete Strip Texture2D content");

            if (_texture.Height % size.Y != 0)
                throw new Exception("Framesize does not complete Strip Texture2D content");

            int widthFrameNumber = _texture.Width / size.X;
            int heightFrameNumber = _texture.Height / size.Y;
            int _length = widthFrameNumber * heightFrameNumber;

            if (_orientation == StripOrientation.LeftToRight && heightFrameNumber > 1)
                throw new Exception("TextureStrip orientation does not match content size");

            if (_orientation == StripOrientation.TopToBottom && widthFrameNumber > 1)
                throw new Exception("TextureStrip orientation does not match content size");

            rectangleFrames = new Rectangle[_length];

            //Create matchup between index in rectangleFrames and actual Rectangles in the texture.
            switch (_orientation)
            {
                case StripOrientation.LeftToRight:

                    for (int i = 0; i < widthFrameNumber; i++)
                        rectangleFrames[i] = new Rectangle(i * size.X, 0, size.X,size.Y);

                    break;
                case StripOrientation.TopToBottom:

                    for (int i = 0; i < heightFrameNumber; i++)
                        rectangleFrames[i] = new Rectangle(0, i*size.Y, size.X, size.Y);

                    break;
                case StripOrientation.LeftToRightAndTopToBottom:

                    for(int i=0;i<heightFrameNumber;i++)
                        for (int j = 0; j < widthFrameNumber; j++)
                            rectangleFrames[i * widthFrameNumber + j] = new Rectangle(j * size.X, i * size.Y, size.X, size.Y);

                    break;
                case StripOrientation.TopToBottomAndLeftToRight:

                    for (int i = 0; i < widthFrameNumber; i++)
                        for (int j = 0; j < heightFrameNumber; j++)
                            rectangleFrames[i * heightFrameNumber + j] = new Rectangle(i * size.X, j * size.Y, size.X, size.Y);

                    break;
            }
        }

        public Rectangle getRectangleFrame(int index)
        {
            return rectangleFrames[index % rectangleFrames.Length];
        }

        public AssetTextureStrip createAssetCopy(String name)
        {
            Texture2D contentCopy = new Texture2D(_texture.GraphicsDevice, _texture.Width, _texture.Height);

            Color[] content = new Color[_texture.Width * _texture.Height];
            _texture.GetData<Color>(content);
            contentCopy.SetData<Color>(content);

            AssetTextureStrip copy = new AssetTextureStrip(name,contentCopy, _orientation, _size);
            return copy;
        }

        public static AssetTextureStrip createFromAssetTexture(AssetTexture texture, String name)
        {
            Texture2D content = texture.createAssetCopy("volatile" + texture.name).texture;

            AssetTextureStrip strip = new AssetTextureStrip(name, content, StripOrientation.LeftToRight, texture.size);
            return strip;
        }
    }
}
