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
using SketchyGraph.GraphClasses;


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
        List<Samples> samples_plot = new List<Samples>();
        List<Samples> samples = new List<Samples>();
        List<BaseGraph> graphs = new List<BaseGraph>();
        bool flagchart = false;
        List<Samples> pieNameSamples = new List<Samples>();
        bool edgeLast = false;
        bool multipleSlice = false;
        bool prepareToSetName = false;
        double extraspace_chart = 40.0;
        string potentialPieObjectName = "";
        bool movingMode = false;
        bool secondPass = false;
        Circle selectedCircle;
        PieChart selectedChart;
        Line selectedLine = new Line();

        public MainWindow()
        {
            InitializeComponent();
            Stylus.SetIsPressAndHoldEnabled(PaperInk, false);
            PaperInk.AddHandler(InkCanvas.MouseDownEvent, new MouseButtonEventHandler(PaperInk_MouseDown), true);
            PaperInk.AddHandler(InkCanvas.StylusDownEvent, new StylusDownEventHandler(PaperInk_StylusDown), true);
            PaperInk.AddHandler(InkCanvas.TouchDownEvent, new RoutedEventHandler(PaperInk_TouchDown), true);
            PaperInk.AddHandler(InkCanvas.TouchUpEvent, new RoutedEventHandler(PaperInk_TouchUp), true);
            PaperInk.DefaultDrawingAttributes = _regularPen;
            PaperInk.EditingMode = InkCanvasEditingMode.Ink;
            PaperInk.DefaultDrawingAttributes.Color = Colors.Black;
            this.samples_graphs = Utils.ReadFiles(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\samples\\graphs\\");
            this.samples_letters = Utils.ReadFiles(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\samples\\letters\\");
            this.samples_numbers = Utils.ReadFiles(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\samples\\numbers\\");
            this.samples_symbols = Utils.ReadFiles(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\samples\\symbols\\");
            this.samples_plot = Utils.ReadFiles(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\samples\\plot\\");

            this.samples.AddRange(this.samples_graphs);
            this.samples.AddRange(this.samples_letters);
            this.samples.AddRange(this.samples_numbers);
            this.samples.AddRange(this.samples_symbols);

            pieNameSamples.AddRange(this.samples_letters);

            debugtxt.FontSize = 35;
        }

        private DrawingAttributes _regularPen =
            new DrawingAttributes
            {
                Width = 5,
                Height = 5
            };

        #region ExtraHandlers

        public void PaperInk_TouchDown(object sender, RoutedEventArgs e)
        {
            Point mouseP = Mouse.GetPosition(PaperInk);
            if (movingMode == true)
            {
                PaperInk.EditingMode = InkCanvasEditingMode.None;
                secondPass = true;
            }
            else
            {
                foreach (BaseGraph bgraph in graphs)
                {
                    if (bgraph.type == "PieChart")
                    {
                        int i = 0;
                        foreach (Circle grbC in ((PieChart)bgraph).grabbingCircles)
                        {
                            Point topLeft = new Point(grbC.Center.X - grbC.Radius, grbC.Center.Y - grbC.Radius);
                            Point BottomRight = new Point(grbC.Center.X + grbC.Radius, grbC.Center.Y + grbC.Radius);
                            Rect x = new Rect(topLeft, BottomRight);

                            if (mouseP.X >= topLeft.X && mouseP.Y >= topLeft.Y && mouseP.X <= BottomRight.X && mouseP.Y <= BottomRight.Y)
                            {
                                PaperInk.EditingMode = InkCanvasEditingMode.None;
                                movingMode = true;
                                selectedCircle = grbC;
                                selectedLine = ((PieChart)bgraph).GetSliceLines()[i];
                                selectedChart = ((PieChart)bgraph);
                            }
                            i++;
                        }
                    }
                }
            }
            
            PaperInk.EditingMode = InkCanvasEditingMode.Ink;
        }

        public void PaperInk_TouchUp(object sender, RoutedEventArgs e)
        {
            if (movingMode == true && secondPass == true)
            {
                Point mouseP = Mouse.GetPosition(PaperInk);
                
                Line newLine = new Line();
                newLine.X1 = selectedLine.X1;
                newLine.Y1 = selectedLine.Y1;
                newLine.X2 = mouseP.X;
                newLine.Y2 = mouseP.Y;

                Vector v1 = new Vector(newLine.X1 - newLine.X2, newLine.Y1 - newLine.Y2);
                Vector v2 = new Vector(selectedLine.X1 - selectedLine.X2, selectedLine.Y1 - selectedLine.Y2);

                Vector checkV;

                double angleNum = Vector.AngleBetween(v1, v2);

                if (angleNum < 0)
                    angleNum = angleNum + 360.0;

                if (angleNum <= 180)
                {
                    checkV = new Vector((-1.0) * (newLine.X1 - newLine.X2), (-1.0) * (newLine.Y1 - newLine.Y2));
                }
                else
                {
                    checkV = new Vector((-1.0) * (newLine.X1 - newLine.X2), (-1.0)*(newLine.Y1 - newLine.Y2));
                }
                double radius = GeneralUtil.EuclideanDistance(new Point(selectedLine.X1, selectedLine.Y1), new Point(selectedLine.X2, selectedLine.Y2));
                checkV.Normalize();
                //RotateTransform rotateTransform = new RotateTransform(angleNum);
                //rotateTransform.CenterX = selectedLine.X1;
                //rotateTransform.CenterY = selectedLine.Y1;
                //selectedLine.RenderTransform = rotateTransform;
                //Geometry el = selectedLine.RenderedGeometry;
                selectedLine.X2 = selectedLine.X1 + (radius * checkV.X);

                selectedLine.Y2 = selectedLine.Y1 + (radius * checkV.Y);

                foreach (BaseGraph bgraph in graphs)
                {
                    if (bgraph.Equals(selectedChart))
                    {
                        List<Line> linList = ((PieChart)bgraph).GetSliceLines();
                        foreach (SliceObject sobJ in ((PieChart)bgraph).GetSliceObjects())
                        {
                            if (sobJ.GetLine1().Equals(selectedLine) || sobJ.GetLine2().Equals(selectedLine))
                                sobJ.manipulated = true;
                        }
                        ((PieChart)bgraph).UpdateLines();
                        UpdateAndRedrawPieStuff((PieChart)bgraph);
                        foreach (SliceObject sobj in ((PieChart)bgraph).GetSliceObjects())
                        {
                            AddOrUpdatePieInformation(sobj, false, true);
                        }
                    }
                }

                movingMode = false;
                secondPass = false;
                selectedLine = new Line();
                selectedCircle = null;
            }
        }

        public void PaperInk_StylusDown(object sender, StylusDownEventArgs e)
        {
            Point mouseP = Mouse.GetPosition(PaperInk);
            Rect rect = PaperInk.GetSelectionBounds();
            if (mouseP.X >= rect.Left && mouseP.X <= (rect.Left + rect.Width) && mouseP.Y >= rect.Top && mouseP.Y <= (rect.Top + rect.Height))
            {
                if (PaperInk.EditingMode == InkCanvasEditingMode.Select && PaperInk.GetSelectedStrokes().Count > 0)
                {
                    StrokeCollection strokeCollection = PaperInk.GetSelectedStrokes();
                    List<Stroke> selectedStrokes = new List<Stroke>();
                    foreach (Stroke strk in strokeCollection)
                    {
                        selectedStrokes.Add(strk);
                    }
                    Tuple<double, string, double> result = Recognizer.RecognizedSelected(selectedStrokes, true, this.pieNameSamples);

                    prepareToSetName = true;
                    this.potentialPieObjectName = result.Item2;
                    Debug.WriteLine("PrepareToSetName = true");
                    PaperInk.EditingMode = InkCanvasEditingMode.Ink;
                }
            }
        }
        public void PaperInk_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mouseP = Mouse.GetPosition(PaperInk);
            Rect rect = PaperInk.GetSelectionBounds();
            if (mouseP.X >= rect.Left && mouseP.X <= (rect.Left + rect.Width) && mouseP.Y >= rect.Top && mouseP.Y <= (rect.Top + rect.Height))
            {
                if (PaperInk.EditingMode == InkCanvasEditingMode.Select && PaperInk.GetSelectedStrokes().Count > 0)
                {
                    StrokeCollection strokeCollection = PaperInk.GetSelectedStrokes();
                    List<Stroke> selectedStrokes = new List<Stroke>();
                    foreach (Stroke strk in strokeCollection)
                    {
                        selectedStrokes.Add(strk);
                    }
                    Tuple<double, string, double> result = Recognizer.RecognizedSelected(selectedStrokes, true, this.pieNameSamples);

                    prepareToSetName = true;
                    this.potentialPieObjectName = result.Item2;
                    Debug.WriteLine("PrepareToSetName = true");
                    PaperInk.EditingMode = InkCanvasEditingMode.Ink;
                }
            }
        }
        #endregion

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
                bool flag = false;

                if (graphs.Count == 0)
                {
                    string el = RealTimeGestureRecognition(e, this.samples);
                }

                foreach (BaseGraph bgraph in graphs)
                {
                    if (bgraph.type == "PieChart" && !bgraph.hasbeendrawn)
                    {
                        //Circle circ = ((PieChart)bgraph).GetCircleArea();

                        DrawCircle(((PieChart)bgraph).GetCircleArea(), Brushes.Black);
                        if (e.Stroke == ((PieChart)bgraph).GetCircumference())
                            PaperInk.Strokes.Remove(e.Stroke);

                        bgraph.hasbeendrawn = true;
                    }
                    else if (bgraph.type == "PieChart" && bgraph.hasbeendrawn)
                    {
                        flag = true;
                    }
                    else if (bgraph.type == "PieChart" && StrokeNotInPieChart(e.Stroke, ((PieChart)bgraph)) == false)
                    {
                        //Debug.WriteLine("Stroke is inside");
                        if (multipleSlice == true)
                        {
                            Line temp1 = new Line();
                            Line temp2 = new Line();

                            DrawLine(temp1, bgraph, e.Stroke, true, true);
                            DrawLine(temp2, bgraph, e.Stroke, true, false);
                            PaperInk.Strokes.Remove(e.Stroke);
                            multipleSlice = false;
                        }
                        else
                        {
                            Line temp = new Line();
                            DrawLine(temp, bgraph, e.Stroke, false, false);
                            PaperInk.Strokes.Remove(e.Stroke);
                            ((PieChart)bgraph).addSlices(e.Stroke);
                        }

                        //If more than 1 slice, run realtime calculation of angles.
                        //Sorting method should be inside PieChart class, for constant sorting whenever slice added.
                    }
                    else if (bgraph.type == "PieChart" && bgraph.hasbeendrawn && StrokeNotInPieChart(e.Stroke, ((PieChart)bgraph)) == true)
                    {
                        if (((PieChart)bgraph).GetSliceObjects().Count > 1)
                        {
                            ColorOrNameSet(((PieChart)bgraph), e.Stroke);
                        }
                    }
                    else if (bgraph.type == "BarChart" && !bgraph.hasbeendrawn)
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
                            List<Samples> y_samples = new List<Samples>();
                            y_samples.AddRange(this.samples_symbols);
                            y_samples.AddRange(this.samples_numbers);

                            bgraph.AddRangeValue(e.Stroke);
                            foreach (RangeValue rv in bgraph.getRangeValuesModified())
                                rv.parse(this.samples_numbers);
                            bgraph.ResetModifiedFlag();
                            ValidateWrongValues(bgraph.validateRangeValues());
                            double b = 10;
                        }
                        else if (area == "plot_bound")
                        {

                            if (e.Stroke.StylusPoints.Count == 1)
                            {
                                double width = 1.0;
                                StylusPointCollection newpoints = new StylusPointCollection();
                                foreach (Element el in bgraph.elements) { 
                                    if(el.isInsidePlot(new Point(e.Stroke.StylusPoints[0].X, e.Stroke.StylusPoints[0].Y))){
                                        Rect bbel = Unistroke.BoundingBox(el.plot.points);
                                        width = bbel.Width;
                                        newpoints.Add(new StylusPoint(bbel.Left + (bbel.Width /2), bbel.Top));
                                        newpoints.Add(new StylusPoint(bbel.Left + (bbel.Width /2), bbel.Bottom));
                                    }
                                }
                                if (newpoints.Count > 0)
                                {
                                    Stroke str = new Stroke(newpoints);
                                    str.DrawingAttributes.Color = Colors.Yellow;
                                    str.DrawingAttributes.Width = width;
                                    str.DrawingAttributes.IsHighlighter = true;
                                    PaperInk.Strokes.Add(str);
                                }
                                else {
                                    //XY Plot
                                }
                                PaperInk.Strokes.Remove(e.Stroke);
                            }
                            else
                            {

                                double max = bgraph.maxRange;
                                string el = RealTimeGestureRecognition(e, this.samples_plot);
                                if (el == "bar")
                                {
                                    double y_e = e.Stroke.GetBounds().Top;
                                    int index = bgraph.getIndexofRangeValues(y_e);
                                    double upper = 0.0;
                                    double lower = 0.0;
                                    double y_max = 0.0;
                                    double y_min = 0.0;
                                    double val = 0.0;
                                    if (index != 0 && index != bgraph.rval.Count)
                                    {
                                        upper = bgraph.rval[index - 1].getNumericalValue();
                                        lower = bgraph.rval[index].getNumericalValue();
                                        y_max = bgraph.rval[index - 1].getBoundingBox().Top;
                                        y_min = bgraph.rval[index].getBoundingBox().Bottom;
                                        val = (((y_e - y_min) / (y_max - y_min)) * (upper - lower)) + lower;
                                    }
                                    else if (index == bgraph.rval.Count)
                                    {
                                        upper = bgraph.rval[bgraph.rval.Count - 1].getNumericalValue();
                                        lower = 0.0;
                                        y_max = bgraph.rval[bgraph.rval.Count - 1].getBoundingBox().Top;
                                        y_min = ((AxisPlot)bgraph).y.GetBounds().BottomLeft.Y;
                                        val = (((y_e - y_min) / (y_max - y_min)) * (upper - lower)) + lower;
                                    }
                                    else
                                    {
                                        val = bgraph.maxRange;
                                    }

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
                        }
                        else
                        {
                            flag = true;
                        }
                    }
                }
                if (flag)
                {
                    string el = RealTimeGestureRecognition(e, this.samples);

                    /*didactic purposes ---- erase later -----*/
                    foreach (BaseGraph bgraph in graphs)
                    {
                        if (bgraph.type == "PieChart" && bgraph.hasbeendrawn && StrokeNotInPieChart(e.Stroke, ((PieChart)bgraph)) == false)
                        {
                            //If True, need to create 2 Lines.
                            if (multipleSlice == true)
                            {
                                Line temp1 = new Line();
                                Line temp2 = new Line();

                                DrawLine(temp1, bgraph, e.Stroke, true, true);
                                DrawLine(temp2, bgraph, e.Stroke, true, false);
                                PaperInk.Strokes.Remove(e.Stroke);
                                multipleSlice = false;
                            }
                            else
                            {
                                Line temp = new Line();
                                DrawLine(temp, bgraph, e.Stroke, false, false);
                                PaperInk.Strokes.Remove(e.Stroke);
                                ((PieChart)bgraph).addSlices(e.Stroke);
                            }

                        }
                        else if (bgraph.type == "PieChart" && bgraph.hasbeendrawn && StrokeNotInPieChart(e.Stroke, ((PieChart)bgraph)) == true)
                        {
                            if (((PieChart)bgraph).GetSliceObjects().Count > 1)
                            {
                                ColorOrNameSet(((PieChart)bgraph), e.Stroke);
                            }
                        }
                        else if (bgraph.type == "BarChart" && !bgraph.hasbeendrawn)
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
                            if (e.Stroke == ((PieChart)bgraph).GetCircumference())
                                PaperInk.Strokes.Remove(e.Stroke);
                            //PaperInk.Strokes.Remove(e.Stroke);
                            bgraph.hasbeendrawn = true;
                        }
                    }
                }
                //debugtxt.Text = selected.Count.ToString();
                //tree = new Node<string>(el);
            }
        }

        public void ColorOrNameSet(PieChart bgraph, Stroke e)
        {
            Debug.WriteLine("Begin hitpoint test");
            foreach (SliceObject sObj in bgraph.GetSliceObjects())
            {
                List<Line> tempLineList = new List<Line>();
                tempLineList = sObj.GetHighLightedLines();
                bool check = false;
                if (sObj.justUpdated == false)
                {
                    AddOrUpdatePieInformation(sObj, false, false);
                    continue;
                }
                foreach (Line tempLine in tempLineList)
                {
                    if (e.HitTest(GetBoundBoxOfLine(tempLine), 5))
                    {
                        Debug.WriteLine("YES, HIT THAT LINE");
                        check = true;
                        break;
                    }
                }
                if (check == true)
                {
                    if (this.prepareToSetName == true)
                    {
                        Debug.WriteLine("Sets Name");
                        AddOrUpdatePieInformation(sObj, false, false);
                        this.potentialPieObjectName = "";
                        
                        prepareToSetName = false;
                    }
                    else
                    {
                        foreach (Line tempLine in tempLineList)
                        {
                            tempLine.Stroke = Brushes.Yellow;
                        }
                        sObj.SetHighlightedLines(tempLineList);
                    }
                    break;
                }
            }
        }

        public void AddOrUpdatePieInformation(SliceObject sObj, bool updateTrue, bool manipulationFinished)
        {
            //if (sObj.dataNameBox
            if (PaperInk.Children.Contains(sObj.dataNameBox))
                PaperInk.Children.Remove(sObj.dataNameBox);
            if (PaperInk.Children.Contains(sObj.dataValBox))
                PaperInk.Children.Remove(sObj.dataValBox);

            if (updateTrue == false && manipulationFinished == false)
            {               
                sObj.SetDataName(this.potentialPieObjectName);
                sObj.dataNameBox.Text = sObj.GetDataName();
                sObj.dataValBox.Text = sObj.GetPercentage() + "%";
                PaperInk.Children.Add(sObj.dataNameBox);
                PaperInk.Children.Add(sObj.dataValBox);
                InkCanvas.SetTop(sObj.dataNameBox, sObj.textLocation.Y);
                InkCanvas.SetLeft(sObj.dataNameBox, sObj.textLocation.X);
                InkCanvas.SetTop(sObj.dataValBox, sObj.dataLocation.Y);
                InkCanvas.SetLeft(sObj.dataValBox, sObj.dataLocation.X);
            }
            else if (manipulationFinished == true)
            {
                sObj.dataNameBox.Text = sObj.GetDataName();
                sObj.dataValBox.Text = sObj.GetPercentage() + "%";
                PaperInk.Children.Add(sObj.dataNameBox);
                PaperInk.Children.Add(sObj.dataValBox);
                InkCanvas.SetTop(sObj.dataNameBox, sObj.textLocation.Y);
                InkCanvas.SetLeft(sObj.dataNameBox, sObj.textLocation.X);
                InkCanvas.SetTop(sObj.dataValBox, sObj.dataLocation.Y);
                InkCanvas.SetLeft(sObj.dataValBox, sObj.dataLocation.X);
            }
        }

        public void ValidateWrongValues(List<RangeValue> rvals)
        {
            double thres = 5;
            foreach (Stroke e in PaperInk.Strokes)
            {
                e.DrawingAttributes.Color = Colors.Black;
                
                foreach (RangeValue rv in rvals)
                {
                    Rect ebb = e.GetBounds();
                    Rect rvb = rv.getBounds();
                    Rect rvbb = new Rect(rvb.Left - thres, rvb.Top - thres, rvb.Width + 2 * thres, rvb.Height + 2 * thres);

                    DrawRectangle(ebb, Brushes.Red);
                    DrawRectangle(rvbb, Brushes.Blue);

                    if (Utils.isInsideRect(rvbb, ebb))
                    {
                        e.DrawingAttributes.Color = Colors.Red;
                    }
                }
            }
        }

        public Rect GetBoundBoxOfLine(Line _element)
        {
            Rect rectangleBounds = new Rect();
            Line x = new Line();
            rectangleBounds = x.RenderTransform.TransformBounds(new Rect(new Point(_element.X1, _element.Y1), new Point(_element.X2, _element.Y2)));
            return rectangleBounds;
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
                if (val > area.Radius)
                {
                    isOutside = true;
                    Debug.WriteLine("Stroke Outside");
                    break;
                }
            }

            if (PieUtil.trueSlice(arg, chart, area.Radius))
            {
                Debug.WriteLine("Time to add a slice");
                isOutside = false;
                this.multipleSlice = true;
            }
            else
            {
                Debug.WriteLine("Check Single Cut");
                startPoint = GeneralUtil.EuclideanDistance(arg[0], area.Center);
                endPoint = GeneralUtil.EuclideanDistance(arg[arg.Count() - 1], area.Center);
                startRatio = PieUtil.FindStartRatio(area.Radius, startPoint);
                endRatio = PieUtil.FindEndRatio(area.Radius, endPoint);

                //Checks whether or not a stroke stretches from the edge to the center point
                if (startRatio >= varianceStart && endRatio <= varianceEnd)
                {
                    isOutside = false;
                    edgeLast = false;
                }
                //Inversely, this checks to see if the strokes was drawn from the center to the edge.
                else if(startRatio <= varianceEnd && endRatio >= varianceStart)
                {
                    isOutside = false;
                    edgeLast = true;
                }
                //If neither of the above statements are true, then it is not an accurate stroke or
                //
                else
                {
                    isOutside = true;
                }
            }
            
            if (isOutside == true)
            {
                if (PieUtil.IsInsideCheck(arg, chart.GetBoundingBox()))
                {
                    PaperInk.Strokes.Remove(e);
                }
            }

            return isOutside;
        }

        /*
         * Method used to draw a Pie Chart line representing the line slice inside a pie chart.
         */
        public void DrawLine(Line line, BaseGraph bgraph, Stroke e, bool multiStrokeTrue, bool multiStrokeStart)
        {
            int sizeStrokeList = e.StylusPoints.Count;
            if (multiStrokeTrue && multiStrokeStart)
            {
                line.X1 = ((PieChart)bgraph).GetCenterPoint().X;
                line.Y1 = ((PieChart)bgraph).GetCenterPoint().Y;
                line.X2 = e.StylusPoints[0].X;
                line.Y2 = e.StylusPoints[0].Y; 
            }
            else if (multiStrokeTrue && (multiStrokeStart == false))
            {
                line.X1 = ((PieChart)bgraph).GetCenterPoint().X;
                line.Y1 = ((PieChart)bgraph).GetCenterPoint().Y;
                line.X2 = e.StylusPoints[sizeStrokeList - 1].X;
                line.Y2 = e.StylusPoints[sizeStrokeList - 1].Y;
            }
            else if (edgeLast == true)
            {
                line.X1 = ((PieChart)bgraph).GetCenterPoint().X;
                line.Y1 = ((PieChart)bgraph).GetCenterPoint().Y;
                line.X2 = e.StylusPoints[sizeStrokeList - 1].X;
                line.Y2 = e.StylusPoints[sizeStrokeList - 1].Y;
                edgeLast = false;
            }
            else
            {
                line.X1 = ((PieChart)bgraph).GetCenterPoint().X;
                line.Y1 = ((PieChart)bgraph).GetCenterPoint().Y;
                line.X2 = e.StylusPoints[0].X;
                line.Y2 = e.StylusPoints[0].Y;
            }
            line.IsHitTestVisible = true;
            ((PieChart)bgraph).addSlices(line);

            if (((PieChart)bgraph).holdingList.Count > 1)
            {
                foreach (SliceObject sliceObjHold in ((PieChart)bgraph).holdingList)
                {
                    if (sliceObjHold.tagged == false)
                    {
                        List<Line> cleanup = sliceObjHold.GetHighLightedLines();
                        foreach (Line ln in cleanup)
                        {
                            ln.Stroke = Brushes.White;
                        }
                        AddOrUpdatePieInformation(sliceObjHold, true, false);
                    }
                    sliceObjHold.tagged = false;
                }
            }

            UpdateAndRedrawPieStuff(((PieChart)bgraph));
        }

        public void UpdateAndRedrawPieStuff(PieChart bgraph)
        {
            if (bgraph.GetModifiedStatus())
            {
                List<List<Line>> tempList = new List<List<Line>>();
                foreach (SliceObject sObj in (bgraph.GetSliceObjects()))
                {
                    tempList.Add(sObj.GetHighLightedLines());
                }
                foreach (List<Line> lineList in tempList)
                {
                    foreach (Line lineObj in lineList)
                    {
                        if (PaperInk.Children.Contains(lineObj))
                        {
                            lineObj.Stroke = Brushes.White;
                            PaperInk.Children.Remove(lineObj);
                        }
                    }
                }

            }
            if (bgraph.GetSliceObjects().Count > 1)
            {
                foreach (SliceObject sObj in (bgraph.GetSliceObjects()))
                {
                    foreach (Line lineObject in sObj.GetHighLightedLines())
                    {
                        PaperInk.Children.Add(lineObject);
                    }

                    if (sObj.manipulated == true)
                        sObj.manipulated = false;

                }
                bgraph.SetModifiedStatus(false);
            }

            //Redraw Slice Lines
            foreach (Line lineSlice in bgraph.GetSliceLines())
            {
                if (PaperInk.Children.Contains(lineSlice))
                    PaperInk.Children.Remove(lineSlice);

                lineSlice.Stroke = Brushes.Black;
                lineSlice.StrokeThickness = 3;
                PaperInk.Children.Add(lineSlice);
            }
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

            Tuple<List<Stroke>, List<int>> check = Utils.RobustIntersection(e.Stroke, selected);
            if (check.Item1.Count == 1)
            {
                while (selected.Count != 0)
                    selected.RemoveAt(0);
            }
            selected.Add(e.Stroke);

            StrokeCollection selectCollect = new StrokeCollection(selected);

            if (selected.Count == 1)
            {
                Tuple<double, string, double> result = Recognizer.RecognizedSelected(selected, true, samples);


                if (result.Item1 > 0.75)
                {
                    if (result.Item2 == "piechart")
                    {
                        //FeedbackTextbox(e, result);
                        PieChart piechart = new PieChart(e.Stroke);
                        piechart.type = "PieChart";
                        graphs.Add(piechart);
                    }
                    else if (result.Item2 == "barchart")
                    {
                        bool insideCheck = false;
                        foreach (BaseGraph grph in graphs)
                        {
                            if (grph.type == "PieChart")
                            {
                                Rect pie = ((PieChart)grph).GetCircumference().GetBounds();
                                Rect selectB = selectCollect.GetBounds();
                                if (pie.TopLeft.X < selectB.TopLeft.X && pie.TopLeft.Y < selectB.TopLeft.Y
                                    && pie.BottomRight.X > selectB.BottomRight.X && pie.BottomRight.Y > selectB.BottomRight.Y)
                                {
                                    insideCheck = true;
                                    break;
                                }
                            }
                        }
                        if (insideCheck == false)
                        {
                            FeedbackTextbox(e, result);
                            // delete from selected the one that is already recognized on this context.
                            BarChart barchart = new BarChart(e.Stroke);
                            barchart.type = "BarChart";
                            graphs.Add(barchart);
                        }
                    }
                    else
                        debugtxt.Text = result.Item2;
                    val = result.Item2;
                }
                else
                    debugtxt.Text = "Non recognized";

                //val = result.Item2;
            }
            else if (selected.Count > 1)
            {
                Stroke first = selected[0];
                selected.RemoveAt(0);
                Tuple<List<Stroke>, List<int>> temp = Utils.RobustIntersection(first, selected);
                Tuple<double, string, double> result = Recognizer.RecognizedSelected(temp.Item1, true, samples);

                if (result.Item1 > 0.75)
                {
                    if (result.Item2 == "barchart")
                    {
                        bool insideCheck = false;
                        foreach (BaseGraph grph in graphs)
                        {
                            if (grph.type == "PieChart")
                            {
                                Rect pie = ((PieChart)grph).GetCircumference().GetBounds();
                                Rect selectB = selectCollect.GetBounds();
                                if (pie.TopLeft.X < selectB.TopLeft.X && pie.TopLeft.Y < selectB.TopLeft.Y
                                    && pie.BottomRight.X > selectB.BottomRight.X && pie.BottomRight.Y > selectB.BottomRight.Y)
                                {
                                    insideCheck = true;
                                    break;
                                }
                            }
                        }
                        if (insideCheck == false)
                        {
                            FeedbackTextbox(e, result);
                            // delete from selected the one that is already recognized on this context.
                            BarChart barchart = new BarChart(temp.Item1[1], temp.Item1[0]);
                            barchart.type = "BarChart";
                            graphs.Add(barchart);
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
                foreach (Element el in graph.elements)
                    resultstxt.Text += el.domain_val.val + "=" + el.range_val.val + "\n";
                foreach (RangeValue rv in graph.rval)
                    resultstxt.Text += rv.value + "\n";
                resultstxt.Text += "\n";
                if (graph.type == "PieChart")
                {
                    List<SliceObject> slc = ((PieChart)graph).GetSliceObjects();
                    foreach (SliceObject sObj in slc)
                    {
                        AddOrUpdatePieInformation(sObj, false, false);
                    }
                    //AddOrUpdatePieInformation(SliceObject sObj, bool updateTrue, bool manipulationFinished)
                }
            }
        }

    }
}
