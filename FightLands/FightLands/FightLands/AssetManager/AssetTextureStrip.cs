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
        Point frameSize;
        Texture2D content;
        StripOrientation orientation;
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
            this.content = content;
            this.orientation = orientation;
            this.frameSize = frameSize;

            UpdateRectangleFrames();
        }

        private void UpdateRectangleFrames()
        {
            if (content.Width == 0 || content.Height == 0)
                throw new Exception("Invalid texture format");

            if (content.Width % frameSize.X != 0)
                throw new Exception("Framesize does not complete Strip Texture2D content");

            if (content.Height % frameSize.Y != 0)
                throw new Exception("Framesize does not complete Strip Texture2D content");

            int widthFrameNumber = content.Width / frameSize.X;
            int heightFrameNumber = (content.Height % frameSize.Y);
            _length = widthFrameNumber * heightFrameNumber;

            if (orientation == StripOrientation.LeftToRight && heightFrameNumber > 1)
                throw new Exception("TextureStrip orientation does not match content size");

            if (orientation == StripOrientation.TopToBottom && widthFrameNumber > 1)
                throw new Exception("TextureStrip orientation does not match content size");

            rectangleFrames = new Rectangle[_length];

            //Create matchup between index in rectangleFrames and actual Rectangles in the texture.
            switch (orientation)
            {
                case StripOrientation.LeftToRight:

                    for (int i = 0; i < widthFrameNumber; i++)
                        rectangleFrames[i] = new Rectangle(i * frameSize.X, 0, frameSize.X,frameSize.Y);

                    break;
                case StripOrientation.TopToBottom:

                    for (int i = 0; i < heightFrameNumber; i++)
                        rectangleFrames[i] = new Rectangle(0, i*frameSize.Y, frameSize.X, frameSize.Y);

                    break;
                case StripOrientation.LeftToRightAndTopToBottom:

                    for(int i=0;i<heightFrameNumber;i++)
                        for (int j = 0; j < widthFrameNumber; j++)
                            rectangleFrames[i * widthFrameNumber + j] = new Rectangle(j * frameSize.X, i * frameSize.Y, frameSize.X, frameSize.Y);

                    break;
                case StripOrientation.TopToBottomAndLeftToRight:

                    for (int i = 0; i < widthFrameNumber; i++)
                        for (int j = 0; j < heightFrameNumber; j++)
                            rectangleFrames[i * heightFrameNumber + j] = new Rectangle(i * frameSize.X, j * frameSize.Y, frameSize.X, frameSize.Y);

                    break;
            }
        }
    }
}
