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
using System.IO;
using System.Windows.Ink;

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

        public MainWindow()
        {
            InitializeComponent();
            mainDrawingCanvas.DefaultDrawingAttributes = _regularPen;
            mainDrawingCanvas.EditingMode = InkCanvasEditingMode.Ink;
            mainDrawingCanvas.DefaultDrawingAttributes.Color = Colors.Black;
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
                            mainDrawingCanvas.Strokes.Save(fs);
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
                        mainDrawingCanvas.Strokes.Save(fs);
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
                mainDrawingCanvas.DefaultDrawingAttributes = _regularPen;
                mainDrawingCanvas.EditingMode = InkCanvasEditingMode.Ink;
                mainDrawingCanvas.DefaultDrawingAttributes.Color = Colors.Black;

                if (sender.Equals(NewFileTB) || sender.Equals(menuNewBlank))
                {
                    mainDrawingCanvas.Background = Brushes.White;
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
                mainDrawingCanvas.Strokes.Clear();
                mainDrawingCanvas.Children.Clear();
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
                        mainDrawingCanvas.Strokes.Save(fs);
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
                        mainDrawingCanvas.Strokes = strokes;
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
                mainDrawingCanvas.DefaultDrawingAttributes = _regularPen;
                mainDrawingCanvas.EditingMode = InkCanvasEditingMode.Ink;
                mainDrawingCanvas.DefaultDrawingAttributes.Color = Colors.Black;
            }
            else if (sender.Equals(pointErase))
            {
                mainDrawingCanvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
            }
            else if (sender.Equals(strokeErase))
            {
                mainDrawingCanvas.EditingMode = InkCanvasEditingMode.EraseByStroke;
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
            mainDrawingCanvas.EditingMode = InkCanvasEditingMode.Select;
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

            mainDrawingCanvas.Background = drawBrush;
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
                mainDrawingCanvas.Background = backgndBrush;
            }
        }
        #endregion
    }
}
