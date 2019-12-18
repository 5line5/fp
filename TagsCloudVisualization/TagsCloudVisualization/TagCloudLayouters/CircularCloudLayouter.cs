﻿using ResultPatterLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TagsCloudVisualization.Geometry;
using TagsCloudVisualization.Settings;

namespace TagsCloudVisualization.TagCloudLayouters
{
    public class CircularCloudLayouter: ILayouter
    {
        private readonly List<Rectangle> rectangles = new List<Rectangle>();
        private readonly SortedList<double, HashSet<Point>> corners = new SortedList<double, HashSet<Point>>();
        private readonly CloudSettings cloudSettings;

        public CircularCloudLayouter(CloudSettings cloudSettings)
        {
            this.cloudSettings = cloudSettings;
            corners.Add(0, new HashSet<Point> { cloudSettings.Center });
        }

        public Result<Rectangle> PutNextRectangle(Size rectangleSize)
        {
            if (rectangleSize.Width <= 0 || rectangleSize.Height <= 0)
                return Result.Fail<Rectangle>($"This rectangle won't be added, because width and height " +
                                              $"should be greater than 0. Your size was: {rectangleSize}")
                             .OnFail(error => Console.WriteLine(error));

            foreach (var corner in corners.Values)
            {
                foreach (var rectangle in RectangleGeometry.GetCornerRectangles(rectangleSize, corner))
                {
                    if (rectangles.Any(rec => rec.IntersectsWith(rectangle)))
                        continue;
                    if (rectangle.AnyRectanglePointOutOfRange(cloudSettings.Center, cloudSettings.Radius))
                        continue;
                    rectangles.Add(rectangle);
                    AddPointsIntoList(rectangle.GetCorners());
                    return Result.Ok(rectangle);
                }
            }

            return Result.Fail<Rectangle>("This rectangle won't be added, because we can't find the place " +
                                          "to add your rectangle, because rectangle was out of permissible" +
                                          " range :(. Your size was: {rectangleSize}")
                         .OnFail(error => Console.WriteLine(error));    
        }

        private void AddPointsIntoList(IEnumerable<Point> points)
        {
            foreach (var point in points)
            {
                var distance = point.DistanceTo(cloudSettings.Center);
                if (corners.ContainsKey(distance))
                    corners[distance].Add(point);
                else
                    corners.Add(distance, new HashSet<Point>() { point });
            }
        }
    }
}   
