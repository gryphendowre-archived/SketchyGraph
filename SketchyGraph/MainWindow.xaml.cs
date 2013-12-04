using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Ink;
using System.IO;
using System.Reflection;
using SketchyGraph.GraphClasses.Charts;
using System.Diagnostics;
using SketchyGraph.Util;


namespace SketchyGraph
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        String filename = null;
        FileStream fs = null;
        Microsoft.Win32.SaveFileDialog saveDlg = new Microsoft.Win32.SaveFileDialog();
        Microsoft.Win32.OpenFileDialog loadDlg = new Microsoft.Win32.OpenFileDialog();
        List<Stroke> selected = new List<Stroke>();
        List<Samples> samples_graphs = new List<Samples>();
        List<Samples> samples_letters = new List<Samples>();
        List<Samples> samples_numbers = new List<Samples>();
        List<Samples> samples_symbols = new List<Samples>();
        List<Samples> samples = new List<Samples>();
        List<BaseGraph> graphs = new List<BaseGraph>();
        bool flagchart = false;
        double extraspace_chart = 30.0;

        public MainWindow()
        {
            InitializeComponent();
            PaperInk.DefaultDrawingAttributes = _regularPen;
            PaperInk.EditingMode = InkCanvasEditingMode.Ink;
            PaperInk.DefaultDrawingAttributes.Color = Colors.Black;
            this.samples_graphs = Utils.ReadFiles(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\samples\\graphs\\");
            this.samples_letters = Utils.ReadFiles(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\samples\\letters\\");
            this.samples_numbers = Utils.ReadFiles(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\samples\\numbers\\");
            this.samples_symbols = Utils.ReadFiles(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\samples\\symbols\\");

            this.samples.AddRange(this.samples_graphs);
            this.samples.AddRange(this.samples_letters);
            this.samples.AddRange(this.samples_numbers);
            this.samples.AddRange(this.samples_symbols);

            debugtxt.FontSize = 35;
        }

        private DrawingAttributes _regularPen =
            new DrawingAttributes
            {
                Width = 5,
                Height = 5
            };

        #region Menu Options
        public void menuOptionClick(object sender, RoutedEventArgs e)
        {
            if (sender.Equals(menuSave) || sender.Equals(SaveFileTB))
            {
                if (this.filename == null)
                {
                    saveDlg.FileName = "Untitled";
                    saveDlg.DefaultExt = ".wjs";
                    saveDlg.Filter = "Journal Stroke Files (.wjs)|*.wjs";
                    Nullable<bool> result = saveDlg.ShowDialog();

                    if (result == true)
                    {
                        this.filename = saveDlg.FileName;
                        try
                        {
                            fs = new FileStream(this.filename, FileMode.Create);
                            PaperInk.Strokes.Save(fs);
                        }
                        finally
                        {
                            if (fs != null)
                            {
                                fs.Close();
                            }
                        }
                    }
                }
                else
                {
                    try
                    {
                        fs = new FileStream(this.filename, FileMode.Create);
                        PaperInk.Strokes.Save(fs);
                    }
                    finally
                    {
                        if (fs != null)
                        {
                            fs.Close();
                        }
                    }
                }
            }
            else if (sender.Equals(NewFileTB) || sender.Equals(menuNewBlank) || sender.Equals(menuNewGrid) || sender.Equals(menuNewImageBkgnd))
            {
                PaperInk.DefaultDrawingAttributes = _regularPen;
                PaperInk.EditingMode = InkCanvasEditingMode.Ink;
                PaperInk.DefaultDrawingAttributes.Color = Colors.Black;

                if (sender.Equals(NewFileTB) || sender.Equals(menuNewBlank))
                {
                    PaperInk.Background = Brushes.White;
                }
                else if (sender.Equals(menuNewGrid))
                {
                    DrawGridInkCanvasBackground();
                }
                else if (sender.Equals(menuNewImageBkgnd))
                {
                    importCanvasBackground();
                }
                this.filename = null;
                PaperInk.Strokes.Clear();
                PaperInk.Children.Clear();
                graphs.Clear();
            }
            else if (sender.Equals(menuSaveAs))
            {
                if (this.filename != null)
                {
                    saveDlg.FileName = this.filename;
                }
                else
                {
                    saveDlg.FileName = "Untitled";
                }
                saveDlg.DefaultExt = ".wjs";
                saveDlg.Filter = "Journal Stroke Files (.wjs)|*.wjs";
                Nullable<bool> result = saveDlg.ShowDialog();

                /* If 'save' is hit, then result = true. If 'cancel' is hit, then result = false. */
                if (result == true)
                {
                    this.filename = saveDlg.FileName;
                    try
                    {
                        fs = new FileStream(this.filename, FileMode.Create);
                        PaperInk.Strokes.Save(fs);
                    }
                    finally
                    {
                        if (fs != null)
                        {
                            fs.Close();
                        }
                    }
                }
            }
            else if (sender.Equals(menuLoad))
            {
                loadDlg.DefaultExt = ".wjs";
                loadDlg.Filter = "Journal Stroke Files (.wjs)|*.wjs";
                Nullable<bool> result = loadDlg.ShowDialog();
                if (result == true)
                {
                    this.filename = loadDlg.FileName;
                    try
                    {
                        var fs = new FileStream(loadDlg.FileName, FileMode.Open, FileAccess.Read);
                        StrokeCollection strokes = new StrokeCollection(fs);
                        PaperInk.Strokes = strokes;
                    }
                    finally
                    {
                        if (fs != null)
                        {
                            fs.Close();
                        }
                    }
                }
            }
            else if (sender.Equals(BlackPen))
            {
                PaperInk.DefaultDrawingAttributes = _regularPen;
                PaperInk.EditingMode = InkCanvasEditingMode.Ink;
                PaperInk.DefaultDrawingAttributes.Color = Colors.Black;
            }
            else if (sender.Equals(pointErase))
            {
                PaperInk.EditingMode = InkCanvasEditingMode.EraseByPoint;
            }
            else if (sender.Equals(strokeErase))
            {
                PaperInk.EditingMode = InkCanvasEditingMode.EraseByStroke;
            }
            else if (sender.Equals(menuQuit))
            {
                this.Close();
            }

        }

        /*
         * Method enters the select mode, selecting ink.
         */
        private void lassoButton_Click(object sender, RoutedEventArgs e)
        {
            PaperInk.EditingMode = InkCanvasEditingMode.Select;
        }

        #endregion

        #region Background Types
        /*
         * Drawing method to create the Grid background
         */
        private void DrawGridInkCanvasBackground()
        {
            DrawingBrush drawBrush = new DrawingBrush();
            GeometryDrawing drawing = new GeometryDrawing();
            GeometryGroup drawingGroup = new GeometryGroup();
            LineGeometry line = new LineGeometry();
            LineGeometry line2 = new LineGeometry();
            Pen stroke = new Pen();

            Background = Brushes.White;

            line.StartPoint = new Point(0, 0);
            line2.StartPoint = new Point(0, 0);
            line.EndPoint = new Point(5, 0);
            line2.EndPoint = new Point(0, 5);

            drawingGroup.Children.Add(line);
            drawingGroup.Children.Add(line2);

            drawing.Geometry = drawingGroup;
            drawing.Pen = new Pen();
            drawing.Pen.Brush = Brushes.Blue;
            drawing.Pen.Thickness = 0.1;

            drawBrush.Drawing = drawing;
            drawBrush.Viewport = new Rect(0, 12, 20, 20);
            drawBrush.ViewportUnits = BrushMappingMode.Absolute;
            drawBrush.ViewboxUnits = BrushMappingMode.RelativeToBoundingBox;
            drawBrush.TileMode = TileMode.Tile;
            drawBrush.Stretch = Stretch.Fill;

            PaperInk.Background = drawBrush;
        }

        /*
         * Method to import an image background onto the Ink Canvas
         */
        private void importCanvasBackground()
        {
            ImageBrush backgndBrush = new ImageBrush();

            Nullable<bool> result = loadDlg.ShowDialog();
            if (result == true)
            {
                backgndBrush.ImageSource = new BitmapImage(new Uri(loadDlg.FileName));
                PaperInk.Background = backgndBrush;
            }
        }
        #endregion
        

        private Tuple<double, string, double> RecognizedSelected(List<Stroke> temp, bool print, List<Samples> samples)
        {
            double score = 0.75;
            List<Unistroke> sel = Utils.TransformStrokesToUnistrokes(temp);
            Unistroke points = Trazo.Combine_Strokes(sel);
            if (points.points.Count > 3)
            {
                points.points = points.Resample(points.points, 96);
                double w = points.IndicativeAngle(points.points);
                points.points = points.RotateBy(points.points, -w);
                points.points = points.ScaleDimTo(points.points, Unistroke.SIZE, points.d);
                points.points = points.CheckRestoreOrientation(points.points, +w);
                points.points = points.TranslateTo(points.points, points.O);
                points.vector = points.CalcStartUnitVector(points.points, points.I);
            }
            DrawSampledPoints(points.points, Colors.Red, 1);
            Tuple<Unistroke, double, string, double> result = Trazo.Recognize(points, points.vector, sel.Count, samples);
            DrawSampledPoints(result.Item1.points, Colors.Blue, 1);
            //textscore.Text = "Score: " + result.Item2.ToString() + "\n" + "Symbol: " + result.Item3.ToString();
            return new Tuple<double, string, double>(result.Item2, result.Item3, score);
        }

        private void PaperInk_StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            Straw puntos = new Straw(e);
            //Matrix a = TransformListPointsToMatrix(puntos.getPoints());
            List<Point> resampled = puntos.Sampling();
            List<int> corners = puntos.GetCorners(resampled);
            Gesture gest = new Gesture(puntos);

            if (gest.RecognizeGesture(resampled, corners) == "Scribble")
            {
                for (int i = 0; i < PaperInk.Strokes.Count; i++)
                    if (e.Stroke.HitTest(PaperInk.Strokes[i].GetBounds(), 10))
                    {
                        selected.Remove(PaperInk.Strokes[i]);
                        PaperInk.Strokes.RemoveAt(i);
                    }
                PaperInk.Strokes.Remove(e.Stroke);
                debugtxt.Text = "Scribble";
            }
            /*else if (gest.RecognizeGesture(resampled, corners) == "Grouping")
            {
                foreach (Stroke stroke in PaperInk.Strokes)
                {
                    if (e.Stroke.HitTest(stroke.GetBounds(), 1))
                    {
                        if (stroke.DrawingAttributes.Width == 2)
                        {
                            stroke.DrawingAttributes.Color = Colors.Black;
                            stroke.DrawingAttributes.Width = 1;
                            for (int i = 0; i < selected.Count; i++)
                                selected.RemoveAt(i);
                        }
                        else
                        {
                            stroke.DrawingAttributes.Color = Colors.DarkCyan;
                            stroke.DrawingAttributes.Width = 2;
                            if (stroke != e.Stroke)
                                selected.Add(stroke);
                        }
                    }
                }
                PaperInk.Strokes.RemoveAt(PaperInk.Strokes.Count - 1);
            }*/

            //else if (graphs.Count > 0)
            //{
            //    foreach (BaseGraph bgraph in graphs)
            //    {
            //        if (bgraph.type == "PieChart" && StrokeNotInPieChart(e.Stroke,((PieChart)bgraph)) == false)
            //        {
            //            Debug.WriteLine("Stroke is inside");
            //            ((PieChart)bgraph).addSlices(e.Stroke);
            //            Debug.WriteLine("Number of Strokes inside: " + ((PieChart)bgraph).GetSlices().Count);
            //        }
            //    }
            //}
            else
            {
                bool flag = false;

                if (graphs.Count == 0)
                {
                    string el = RealTimeGestureRecognition(e, this.samples);
                }

                foreach (BaseGraph bgraph in graphs)
                {
                    if (bgraph.type == "BarChart" && !bgraph.hasbeendrawn)
                    {
                        bgraph.CalculateBoundingBoxes(extraspace_chart);
                        DrawRectangle(((AxisPlot)bgraph).bb, Brushes.Blue);
                        DrawRectangle(((AxisPlot)bgraph).x_bounds, Brushes.Red);
                        DrawRectangle(((AxisPlot)bgraph).y_bounds, Brushes.Black);
                        DrawRectangle(((AxisPlot)bgraph).plot_bound, Brushes.DarkOrange);
                        bgraph.hasbeendrawn = true;
                    }
                    else if (bgraph.type == "BarChart" && bgraph.hasbeendrawn)
                    {
                        string area = ((BarChart)bgraph).GiveMeAreaChart(e.Stroke.GetBounds());
                        if (area == "x_bounds")
                        {
                            List<Samples> x_samples = new List<Samples>();
                            x_samples.AddRange(this.samples_letters);
                            x_samples.AddRange(this.samples_numbers);
                            string el = RealTimeGestureRecognition(e, x_samples);
                            bool valmodified = false;

                            foreach (Element element in bgraph.elements)
                            {
                                double a = e.Stroke.GetBounds().TopLeft.X + ((e.Stroke.GetBounds().TopRight.X - e.Stroke.GetBounds().TopLeft.X) / 2);
                                if (a > (element.domain_val.x_val - 15.0) && a < (element.domain_val.x_val + 15.0))
                                {
                                    element.domain_val.val = el;
                                    element.domain_val.stroke_val = new Unistroke(Utils.TransformStrokeToListPoints(e.Stroke));
                                    valmodified = true;
                                }
                            }
                            if (!valmodified)
                            {
                                Element elem = new Element();
                                Value v = new Value(el, new Unistroke(Utils.TransformStrokeToListPoints(e.Stroke)));
                                Value w = new Value();
                                elem.domain_val = v;
                                elem.range_val = w;
                                bgraph.addElement(elem);
                            }

                        }
                        else if (area == "y_bounds")
                        {
                            string el = RealTimeGestureRecognition(e, this.samples_numbers);
                            if (el != "")
                                bgraph.maxRange = Convert.ToInt32(el);
                        }
                        else if (area == "plot_bound")
                        {
                            int max = bgraph.maxRange;
                            string el = RealTimeGestureRecognition(e, this.samples_symbols);
                            if (el == "bar")
                            {
                                double y_max = ((AxisPlot)bgraph).y.GetBounds().TopLeft.Y;
                                double y_min = ((AxisPlot)bgraph).y.GetBounds().BottomLeft.Y;
                                double y_e = e.Stroke.GetBounds().TopLeft.Y;
                                double val = ((y_e - y_min) / (y_max - y_min)) * max;

                                foreach (Element element in bgraph.elements)
                                {
                                    double a = e.Stroke.GetBounds().TopLeft.X + ((e.Stroke.GetBounds().TopRight.X - e.Stroke.GetBounds().TopLeft.X) / 2);
                                    if (a > (element.domain_val.x_val - 15.0) && a < (element.domain_val.x_val + 15.0))
                                    {
                                        element.plot = new Unistroke(Utils.TransformStrokeToListPoints(e.Stroke));
                                        element.range_val.val = Convert.ToString(val);
                                    }
                                }
                            }
                        }
                        else
                        {
                            flag = true;
                        }
                    }
                    else if (bgraph.type == "PieChart" && !bgraph.hasbeendrawn)
                    {
                        //Circle circ = ((PieChart)bgraph).GetCircleArea();

                        DrawCircle(((PieChart)bgraph).GetCircleArea(), Brushes.Black);
                        PaperInk.Strokes.Remove(e.Stroke);

                        bgraph.hasbeendrawn = true;

                    }
                    else if (bgraph.type == "PieChart" && bgraph.hasbeendrawn)
                    {
                        flag = true;
                    }
                    else if (bgraph.type == "PieChart" && StrokeNotInPieChart(e.Stroke, ((PieChart)bgraph)) == false)
                    {
                        Debug.WriteLine("Stroke is inside");
                        ((PieChart)bgraph).addSlices(e.Stroke);
                        Debug.WriteLine("Number of Strokes inside: " + ((PieChart)bgraph).GetSlices().Count);
                    }
                }
                if (flag)
                {
                    string el = RealTimeGestureRecognition(e, this.samples);
                    /*didactic purposes ---- erase later -----*/
                    foreach (BaseGraph bgraph in graphs)
                    {
                        if (bgraph.type == "BarChart" && !bgraph.hasbeendrawn)
                        {
                            bgraph.CalculateBoundingBoxes(extraspace_chart);
                            DrawRectangle(((AxisPlot)bgraph).bb, Brushes.Blue);
                            DrawRectangle(((AxisPlot)bgraph).x_bounds, Brushes.Red);
                            DrawRectangle(((AxisPlot)bgraph).y_bounds, Brushes.Black);
                            DrawRectangle(((AxisPlot)bgraph).plot_bound, Brushes.DarkOrange);
                            bgraph.hasbeendrawn = true;
                        }
                        else if (bgraph.type == "PieChart" &&!bgraph.hasbeendrawn)
                        {
                            //Circle circ = ((PieChart)bgraph).GetCircleArea();
                            DrawCircle(((PieChart)bgraph).GetCircleArea(), Brushes.Black);
                            PaperInk.Strokes.Remove(e.Stroke);
                            bgraph.hasbeendrawn = true;
                        }
                        else if (bgraph.type == "PieChart" && bgraph.hasbeendrawn && StrokeNotInPieChart(e.Stroke, ((PieChart)bgraph)) == false)
                        {
                            Debug.WriteLine("Stroke is inside");
                            ((PieChart)bgraph).addSlices(e.Stroke);
                            Debug.WriteLine("Number of Strokes inside: " + ((PieChart)bgraph).GetSlices().Count);
                        }
                    }

                }
                //debugtxt.Text = selected.Count.ToString();
                //tree = new Node<string>(el);
            }
        }

        /**
         * Helper method for determining if a stroke is within a Pie Chart's region
         */ 
        public bool StrokeNotInPieChart(Stroke e, PieChart chart)
        {
            //List<Point> pts = new List<Point>();
            bool isOutside = false;
            double varianceStart = 0.90;
            double varianceEnd = 0.10;
            double startRatio = 0.0;
            double endRatio = 0.0;
            double startPoint = 0.0;
            double endPoint = 0.0;
            StylusPointCollection spc = new StylusPointCollection(e.StylusPoints);
            Point[] arg = (Point[])spc;

            Circle area = chart.GetCircleArea();
            double val = 0;
            foreach (Point pt in arg)
            {
                val = GeneralUtil.EuclideanDistance(pt, area.Center);
                //val = chart.EuclideanDistance(pt, area.Center);
                if (val > area.Radius)
                {
                    isOutside = true;
                    Debug.WriteLine("Stroke Outside");
                    break;
                }
            }

            startPoint = GeneralUtil.EuclideanDistance(arg[0], area.Center);
            endPoint = GeneralUtil.EuclideanDistance(arg[arg.Count()-1], area.Center);
            
            if (startPoint >= area.Radius)
            {
                startRatio = area.Radius / startPoint;
            }
            else
            {
                startRatio = startPoint / area.Radius;
            }
            
            if (endPoint >= area.Radius)
            {
                endRatio = area.Radius / endPoint;
            }
            else
            {
                endRatio = endPoint / area.Radius;
            }

            isMultipleSlice(arg, chart, area.Radius);

            if (startRatio < varianceStart || endRatio > varianceEnd)
            {
                isOutside = true;
            }

            if (isOutside == true)
            {
                PaperInk.Strokes.Remove(e);
            }

            return isOutside;
        }

        /*
         * Helper method to determine if a user directly creates one slice, instead of a straight line in the PieChart
         */ 
        public bool isMultipleSlice(Point[] pts, PieChart chart, double radius)
        {
            bool returnVal = false;
            double totalDist = 0.0;
            double variance = 0.90;
            double doubRad = 2*radius;
            double ratio = 0.0;
            for (int i = 0; i < pts.Count()-1; i++)
            {
                totalDist = totalDist + GeneralUtil.EuclideanDistance(pts[i], pts[i + 1]);
            }

            if (doubRad > totalDist)
            {
                ratio = totalDist/doubRad;
            }
            else
            {
                ratio = doubRad/totalDist;
            }

            if (ratio > variance)
            {
                Debug.WriteLine("Multiple Slice");
                returnVal = true;
            }


            return returnVal;
        }

        /*
         * Method used to draw the rectangles representing the information boxes of a bar/point graph.
         */ 
        public void DrawRectangle(Rect r, Brush brush)
        {
            Rectangle rect = new Rectangle();
            rect.Width = r.Width;
            rect.Height = r.Height;
            rect.Stroke = brush;
            rect.StrokeThickness = 2;
            PaperInk.Children.Add(rect);
            InkCanvas.SetLeft(rect, r.Left);
            InkCanvas.SetTop(rect, r.Top);
        }
        /**
         *  Method that replaces the poor graph drawn by the user with a nicer, cleaner Circle.
         */ 
        public void DrawCircle(Circle c, Brush brush)
        {
            Ellipse ellip = new Ellipse();
            ellip.Width = (c.Radius)*2.0;
            ellip.Height = (c.Radius) * 2.0;
            ellip.Stroke = brush;
            ellip.StrokeThickness = 3;
            PaperInk.Children.Add(ellip);
            InkCanvas.SetLeft(ellip, (c.Center.X - c.Radius));
            InkCanvas.SetTop(ellip, (c.Center.Y - c.Radius));
        }

        /**
         * One part of the meat&Cookies of the project, Gesture Recognition for recognizing the Chart type for SketchyGraph
         */ 
        public string RealTimeGestureRecognition(InkCanvasStrokeCollectedEventArgs e, List<Samples> samples)
        {
            string val = "";
            double thres = 10.0;

            Tuple<List<Stroke>, List<int>> check = Utils.RobustIntersection(e.Stroke, selected);
            if (check.Item1.Count == 1)
            {
                while (selected.Count != 0)
                    selected.RemoveAt(0);
            }
            selected.Add(e.Stroke);

            if (selected.Count == 1)
            {
                Tuple<double, string, double> result = RecognizedSelected(selected, true, samples);

                if (result.Item1 > 0.75)
                {
                    if (result.Item2 == "piechart")
                    {
                        FeedbackTextbox(e, result);
                        PieChart piechart = new PieChart(e.Stroke);
                        piechart.type = "PieChart";
                        graphs.Add(piechart);
                    }
                    else if (result.Item2 == "barchart")
                    {
                        FeedbackTextbox(e, result);
                        // delete from selected the one that is already recognized on this context.
                        BarChart barchart = new BarChart(e.Stroke);
                        barchart.type = "BarChart";
                        graphs.Add(barchart);
                    }
                    else
                        debugtxt.Text = result.Item2;
                }
                else
                    debugtxt.Text = "Non recognized";

                val = result.Item2;
            }
            else if (selected.Count > 1)
            {
                Stroke first = selected[0];
                selected.RemoveAt(0);
                Tuple<List<Stroke>, List<int>> temp = Utils.RobustIntersection(first, selected);
                Tuple<double, string, double> result = RecognizedSelected(temp.Item1, true, samples);

                if (result.Item1 > 0.75)
                {
                    if (result.Item2 == "barchart")
                    {
                        FeedbackTextbox(e, result);
                        // delete from selected the one that is already recognized on this context.
                        BarChart barchart = new BarChart(temp.Item1[1], temp.Item1[0]);
                        barchart.type = "BarChart";
                        graphs.Add(barchart);
                    }
                    else
                    {
                        debugtxt.Text = result.Item2;
                        flagchart = false;
                    }
                }
                else
                    debugtxt.Text = "Non recognized";

                if (result.Item2 == "+" || (temp.Item1.Count >= 2 && temp.Item1.Count < 5))
                    selected.Insert(0, first);
                else
                    if (selected.Count == temp.Item2.Count)
                        for (int j = temp.Item2.Count - 1; j >= 0; j--)
                            selected.RemoveAt(j);
                        //foreach (int j in temp.Item2)
                        //    selected.RemoveAt(j);
                val = result.Item2;
            }
            return val;
        }

        /**
        *  Create the Textbox stating the chart type; Helper Method.
        */
        public void FeedbackTextbox(InkCanvasStrokeCollectedEventArgs e, Tuple<double, string, double> result)
        {
            Rect r = e.Stroke.GetBounds();
            TextBox t = new TextBox();
            t.FontSize = 15;
            t.Width = 200;
            t.Height = 40;
            t.Text = result.Item2;
            t.Visibility = Visibility.Visible;
            InkCanvas.SetLeft(t, r.Left + 100);
            InkCanvas.SetTop(t, r.Top + 100);
            PaperInk.Children.Add(t);
            flagchart = true;
        }
        
        /**
         * Draws Sampled Points for debugging purposes.
         */
        public void DrawSampledPoints(List<Point> resampled, Color c, int i)
        {
            if (resampled.Count > 0)
            {
                PointCollection pt = Utils.TransformListToPointCollection(resampled);
                StylusPointCollection ptss = new StylusPointCollection(pt);
                Stroke news = new Stroke(ptss);
                news.DrawingAttributes.Color = c;
                //var matrix = Matrix.Identity;
                //matrix.Translate(50,50);
                //news.Transform(matrix, true);
                //ResultsInk.Strokes.Add(news);
            }
        }

        /**
         *  UIElement method, calculates the bargraph amount based of its relative position to the Y range.
         */ 
        private void calculate_Click(object sender, RoutedEventArgs e)
        {
            resultstxt.Text = "";
            foreach (BaseGraph graph in this.graphs) {
                resultstxt.Text += graph.type + "\n";
                foreach (Element el in graph.elements) {
                    resultstxt.Text += el.domain_val.val + "=" + el.range_val.val + "\n";
                }
                resultstxt.Text += "\n";
            }
        }

    }
}
