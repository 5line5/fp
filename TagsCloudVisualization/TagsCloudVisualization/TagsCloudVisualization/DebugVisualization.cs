﻿using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TagsCloudVisualization.Settings;

namespace TagsCloudVisualization.TagsCloudVisualization
{
    public class DebugVisualization: ITagsCloudVisualization<Rectangle>
    {
        private readonly ImageSettings imageSettings;
        private readonly List<Rectangle> rectangles;

        public DebugVisualization(ImageSettings imageSettings, List<Rectangle> rectangles)
        {
            this.imageSettings = imageSettings;
            this.rectangles = rectangles;
        }

        public void Draw(Dictionary<string, int> words = default)
        {
            using (var image = new Bitmap(imageSettings.ImageSize.Width, imageSettings.ImageSize.Height))
            {
                using (var drawPlace = Graphics.FromImage(image))
                {
                    var blackPen = new Pen(new SolidBrush(Color.Black), 3);
                    var redPen = new Pen(new SolidBrush(Color.Red), 3);
                    foreach (var rectangle in rectangles)
                    {
                        var rectangleIntersectsAnother = rectangles.Any(rec => rec != rectangle
                                                                               && rectangle.IntersectsWith(rec));
                        drawPlace.DrawRectangle(rectangleIntersectsAnother ? redPen : blackPen, rectangle);
                    }
                }

                image.Save(imageSettings.ImageName + imageSettings.ImageExtention);
            }
        }
    }
}
