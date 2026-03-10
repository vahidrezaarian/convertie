using System.Windows.Media;

namespace Convertie.Assets
{
    internal static class CustomIcons
    {
        public static DrawingImage ScreenHorizontal(Color color)
        {
            var drawingGroup = new DrawingGroup();
            var brush = new SolidColorBrush(color);

            var frameGeometry = Geometry.Parse(
                "M19,4H5C3.9,4 3,4.9 3,6V18C3,19.1 3.9,20 5,20H19C20.1,20 21,19.1 21,18V6C21,4.9 20.1,4 19,4ZM19.5,18.5H4.5V5.5H19.5V18.5Z");
            drawingGroup.Children.Add(new GeometryDrawing(brush, null, frameGeometry));

            var dividerGeometry = Geometry.Parse(
                "M4.5,11.25H19.5V12.75H4.5V11.25Z");
            drawingGroup.Children.Add(new GeometryDrawing(brush, null, dividerGeometry));

            var drawingImage = new DrawingImage(drawingGroup);
            drawingImage.Freeze();

            return drawingImage;
        }

        public static DrawingImage ScreenVertical(Color color)
        {
            var drawingGroup = new DrawingGroup();
            var brush = new SolidColorBrush(color);

            var frameGeometry = Geometry.Parse(
                "M19,4H5C3.9,4 3,4.9 3,6V18C3,19.1 3.9,20 5,20H19C20.1,20 21,19.1 21,18V6C21,4.9 20.1,4 19,4ZM19.5,18.5H4.5V5.5H19.5V18.5Z");
            drawingGroup.Children.Add(new GeometryDrawing(brush, null, frameGeometry));

            var dividerGeometry = Geometry.Parse(
                "M11.25,5.5H12.75V18.5H11.25V5.5Z");
            drawingGroup.Children.Add(new GeometryDrawing(brush, null, dividerGeometry));

            var drawingImage = new DrawingImage(drawingGroup);
            drawingImage.Freeze();

            return drawingImage;
        }

        public static DrawingImage ArrowLeft(Color color)
        {
            var drawingGroup = new DrawingGroup();
            var brush = new SolidColorBrush(color);

            var geometry = Geometry.Parse(
                "M11.7071,4.29289 C12.0976,4.68342 12.0976,5.31658 11.7071,5.70711 L6.41421,11 H20 C20.5523,11 21,11.4477 21,12 C21,12.5523 20.5523,13 20,13 H6.41421 L11.7071,18.2929 C12.0976,18.6834 12.0976,19.3166 11.7071,19.7071 C11.3166,20.0976 10.6834,20.0976 10.2929,19.7071 L3.29289,12.7071 C3.10536,12.5196 3,12.2652 3,12 C3,11.7348 3.10536,11.4804 3.29289,11.2929 L10.2929,4.29289 C10.6834,3.90237 11.3166,3.90237 11.7071,4.29289Z");

            drawingGroup.Children.Add(new GeometryDrawing(brush, null, geometry));

            var drawingImage = new DrawingImage(drawingGroup);
            drawingImage.Freeze();
            return drawingImage;
        }

        public static DrawingImage ArrowUp(Color color)
        {
            var drawingGroup = new DrawingGroup();
            var brush = new SolidColorBrush(color);

            var geometry = Geometry.Parse(
                "M4.29289,11.7071 C4.68342,12.0976 5.31658,12.0976 5.70711,11.7071 L11,6.41421 V20 C11,20.5523 11.4477,21 12,21 C12.5523,21 13,20.5523 13,20 V6.41421 L18.2929,11.7071 C18.6834,12.0976 19.3166,12.0976 19.7071,11.7071 C20.0976,11.3166 20.0976,10.6834 19.7071,10.2929 L12.7071,3.29289 C12.5196,3.10536 12.2652,3 12,3 C11.7348,3 11.4804,3.10536 11.2929,3.29289 L4.29289,10.2929 C3.90237,10.6834 3.90237,11.3166 4.29289,11.7071Z");

            drawingGroup.Children.Add(new GeometryDrawing(brush, null, geometry));

            var drawingImage = new DrawingImage(drawingGroup);
            drawingImage.Freeze();
            return drawingImage;
        }

        public static DrawingImage Copy(Color color)
        {
            var drawingGroup = new DrawingGroup();

            var geometry1 = Geometry.Parse(
                "M16 12.9V17.1C16 20.6 14.6 22 11.1 22H6.9C3.4 22 2 20.6 2 17.1V12.9C2 9.4 3.4 8 6.9 8H11.1C14.6 8 16 9.4 16 12.9Z");
            var drawing1 = new GeometryDrawing(new SolidColorBrush(color), null, geometry1);
            drawingGroup.Children.Add(drawing1);

            var geometry2 = Geometry.Parse(
                "M17.0998 2H12.8998C9.81668 2 8.37074 3.09409 8.06951 5.73901C8.00649 6.29235 8.46476 6.75 9.02167 6.75H11.0998C15.2998 6.75 17.2498 8.7 17.2498 12.9V14.9781C17.2498 15.535 17.7074 15.9933 18.2608 15.9303C20.9057 15.629 21.9998 14.1831 21.9998 11.1V6.9C21.9998 3.4 20.5998 2 17.0998 2Z");
            var drawing2 = new GeometryDrawing(new SolidColorBrush(color), null, geometry2);
            drawingGroup.Children.Add(drawing2);

            var drawingImage = new DrawingImage(drawingGroup);
            drawingImage.Freeze();

            return drawingImage;
        }

        public static DrawingImage Copied(Color color)
        {
            var drawingGroup = new DrawingGroup();

            var geometry1 = Geometry.Parse(
                "M17.0998 2H12.8998C9.81668 2 8.37074 3.09409 8.06951 5.73901C8.00649 6.29235 8.46476 6.75 9.02167 6.75H11.0998C15.2998 6.75 17.2498 8.7 17.2498 12.9V14.9781C17.2498 15.535 17.7074 15.9933 18.2608 15.9303C20.9057 15.629 21.9998 14.1831 21.9998 11.1V6.9C21.9998 3.4 20.5998 2 17.0998 2Z");
            var drawing1 = new GeometryDrawing(new SolidColorBrush(color), null, geometry1);
            drawingGroup.Children.Add(drawing1);

            var geometry2 = Geometry.Parse(
                "M11.1 8H6.9C3.4 8 2 9.4 2 12.9V17.1C2 20.6 3.4 22 6.9 22H11.1C14.6 22 16 20.6 16 17.1V12.9C16 9.4 14.6 8 11.1 8ZM12.29 13.65L8.58 17.36C8.44 17.5 8.26 17.57 8.07 17.57C7.88 17.57 7.7 17.5 7.56 17.36L5.7 15.5C5.42 15.22 5.42 14.77 5.7 14.49C5.98 14.21 6.43 14.21 6.71 14.49L8.06 15.84L11.27 12.63C11.55 12.35 12 12.35 12.28 12.63C12.56 12.91 12.57 13.37 12.29 13.65Z");
            var drawing2 = new GeometryDrawing(new SolidColorBrush(color), null, geometry2);
            drawingGroup.Children.Add(drawing2);

            var drawingImage = new DrawingImage(drawingGroup);
            drawingImage.Freeze();

            return drawingImage;
        }
    }
}
