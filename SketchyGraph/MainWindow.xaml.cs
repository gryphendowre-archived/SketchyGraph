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
            this.samples = Utils.ReadFiles(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\samples\\");
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
        

        private Tuple<double, string, double> RecognizedSelected(List<Stroke> temp, bool print)
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
            Tuple<Unistroke, double, string, double> result = Trazo.Recognize(points, points.vector, sel.Count, this.samples);
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
            else
            {
                string el;
                el = RealTimeGestureRecognition(e);
                
                foreach (BaseGraph bgraph in graphs)
                {
                    if (bgraph.type == "BarChart")
                    {
                        bgraph.CalculateBoundingBoxes(extraspace_chart);
                        
                        DrawRectangle(((AxisPlot)bgraph).bb, Brushes.Blue);
                        DrawRectangle(((AxisPlot)bgraph).x_bounds, Brushes.Red);
                        DrawRectangle(((AxisPlot)bgraph).y_bounds, Brushes.Black);
                        DrawRectangle(((AxisPlot)bgraph).plot_bound, Brushes.DarkOrange);
                    }
                }
                //debugtxt.Text = selected.Count.ToString();
                //tree = new Node<string>(el);
            }
        }

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

        public string RealTimeGestureRecognition(InkCanvasStrokeCollectedEventArgs e)
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
                Tuple<double, string, double> result = RecognizedSelected(selected, true);

                if (result.Item1 > 0.75)
                {
                    if (result.Item2 == "piechart")
                    {
                        Rect r = e.Stroke.GetBounds();
                        TextBox t = new TextBox();
                        t.FontSize = 15;
                        t.Width = 200;
                        t.Height = 40;
                        t.Text = result.Item2;
                        t.Visibility = Visibility.Visible;
                        InkCanvas.SetLeft(t, r.Left + 100);
                        InkCanvas.SetTop(t, r.Top + r.Height + 50);
                        PaperInk.Children.Add(t);
                    }
                    else if (result.Item2 == "barchart")
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
                Tuple<double, string, double> result = RecognizedSelected(temp.Item1, true);

                if (result.Item1 > 0.75)
                {
                    if (result.Item2 == "barchart")
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
                        // delete from selected the one that is already recognized on this context.
                        BarChart barchart = new BarChart(temp.Item1[1], temp.Item1[0]);
                        barchart.type = "BarChart";
                        graphs.Add(barchart);
                    }
                    else if (result.Item2 == "+" && flagchart)
                    {
                        flagchart = false;
                        foreach(UIElement el in PaperInk.Children){
                            if (el is TextBox) {
                                TextBox l = (TextBox)el;
                                l.Text = "GraphXY";
                            }
                        }
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
                        foreach (int j in temp.Item2)
                            selected.RemoveAt(j);
                val = result.Item2;
            }
            return val;
        }

        

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

    }
}
