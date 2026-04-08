using System.Windows.Media;

namespace Convertie.Assets
{
    internal static class CustomIcons
    {
        public static DrawingImage ScreenHorizontal(Color color)
        {
            var drawingGroup = new DrawingGroup();
            var pen = new Pen(new SolidColorBrush(color), 1.0);
            pen.StartLineCap = PenLineCap.Round;
            pen.EndLineCap = PenLineCap.Round;
            pen.LineJoin = PenLineJoin.Round;
            pen.Freeze();

            var outerGeometry = Geometry.Parse(
                "M5.5,3.5 H15.5 C16.6046,3.5 17.5,4.3954 17.5,5.5 V15.5 C17.5,16.6046 16.6046,17.5 15.5,17.5 H5.5 C4.3954,17.5 3.5,16.6046 3.5,15.5 V5.5 C3.5,4.3954 4.3954,3.5 5.5,3.5 Z");
            drawingGroup.Children.Add(new GeometryDrawing(null, pen, outerGeometry));

            var dividerGeometry = Geometry.Parse(
                "M6.5,10.5 H14.5");
            drawingGroup.Children.Add(new GeometryDrawing(null, pen, dividerGeometry));

            var drawingImage = new DrawingImage(drawingGroup);
            drawingImage.Freeze();
            return drawingImage;
        }

        public static DrawingImage ScreenVertical(Color color)
        {
            var drawingGroup = new DrawingGroup();
            var pen = new Pen(new SolidColorBrush(color), 1.0);
            pen.StartLineCap = PenLineCap.Round;
            pen.EndLineCap = PenLineCap.Round;
            pen.LineJoin = PenLineJoin.Round;
            pen.Freeze();

            var outerGeometry = Geometry.Parse(
                "M5.5,3.5 H15.5 C16.6046,3.5 17.5,4.3954 17.5,5.5 V15.5 C17.5,16.6046 16.6046,17.5 15.5,17.5 H5.5 C4.3954,17.5 3.5,16.6046 3.5,15.5 V5.5 C3.5,4.3954 4.3954,3.5 5.5,3.5 Z");
            drawingGroup.Children.Add(new GeometryDrawing(null, pen, outerGeometry));

            var dividerGeometry = Geometry.Parse(
                "M10.5,6.5 V14.5");
            drawingGroup.Children.Add(new GeometryDrawing(null, pen, dividerGeometry));

            var drawingImage = new DrawingImage(drawingGroup);
            drawingImage.Freeze();
            return drawingImage;
        }

        public static DrawingImage Pinned(Color color)
        {
            var drawingGroup = new DrawingGroup();
            var brush = new SolidColorBrush(color);

            var geometry = Geometry.Parse(
                "M19.1835,7.80516 L16.2188,4.83755 C14.1921,2.8089 13.1788,1.79457 12.0904,2.03468 C11.0021,2.2748 10.5086,3.62155 9.5217,6.31506 L8.85373,8.1381 C8.59063,8.85617 8.45908,9.2152 8.22239,9.49292 C8.11619,9.61754 7.99536,9.72887 7.86251,9.82451 C7.56644,10.0377 7.19811,10.1392 6.46145,10.3423 C4.80107,10.8 3.97088,11.0289 3.65804,11.5721 C3.5228,11.8069 3.45242,12.0735 3.45413,12.3446 C3.45809,12.9715 4.06698,13.581 5.28476,14.8 L6.69935,16.2163 L2.22345,20.6964 C1.92552,20.9946 1.92552,21.4782 2.22345,21.7764 C2.52138,22.0746 3.00443,22.0746 3.30236,21.7764 L7.77841,17.2961 L9.24441,18.7635 C10.4699,19.9902 11.0827,20.6036 11.7134,20.6045 C11.9792,20.6049 12.2404,20.5358 12.4713,20.4041 C13.0192,20.0914 13.2493,19.2551 13.7095,17.5825 C13.9119,16.8472 14.013,16.4795 14.2254,16.1835 C14.3184,16.054 14.4262,15.9358 14.5468,15.8314 C14.8221,15.593 15.1788,15.459 15.8922,15.191 L17.7362,14.4981 C20.4,13.4973 21.7319,12.9969 21.9667,11.9115 C22.2014,10.826 21.1954,9.81905 19.1835,7.80516 Z");

            drawingGroup.Children.Add(new GeometryDrawing(brush, null, geometry));

            var drawingImage = new DrawingImage(drawingGroup);
            drawingImage.Freeze();
            return drawingImage;
        }

        public static DrawingImage Unpinned(Color color)
        {
            var drawingGroup = new DrawingGroup();
            var brush = new SolidColorBrush(color);

            var geometry = Geometry.Parse(
                "M1.4694,21.4699 C1.17666,21.763 1.1769,22.2379 1.46994,22.5306 C1.76298,22.8233 2.23786,22.8231 2.5306,22.5301 " +
                "L7.18383,17.8721 C7.47657,17.5791 7.47633,17.1042 7.18329,16.8114 C6.89024,16.5187 6.41537,16.5189 6.12263,16.812 " +
                "L1.4694,21.4699 Z " +

                "M15.4588,5.48026 L18.541,8.56568 L19.6022,7.50556 L16.52,4.42014 L15.4588,5.48026 Z " +
                "M9.26897,18.8989 L5.15229,14.7781 L4.09109,15.8382 L8.20777,19.9591 L9.26897,18.8989 Z " +

                "M17.3032,14.2922 L15.386,15.0125 L15.9136,16.4167 L17.8307,15.6964 L17.3032,14.2922 Z " +
                "M9.03642,8.63979 L9.73087,6.74438 L8.32243,6.22834 L7.62798,8.12375 L9.03642,8.63979 Z " +

                "M6.04438,11.3965 C6.75583,11.2003 7.29719,11.0625 7.73987,10.7438 L6.86346,9.52646 " +
                "C6.69053,9.65097 6.46601,9.72428 5.6457,9.95044 L6.04438,11.3965 Z " +

                "M7.62798,8.12375 C7.33502,8.92332 7.24338,9.14153 7.10499,9.30391 L8.24665,10.2768 " +
                "C8.60041,9.86175 8.7823,9.33337 9.03642,8.63979 L7.62798,8.12375 Z " +

                "M7.73987,10.7438 C7.92696,10.6091 8.09712,10.4523 8.24665,10.2768 L7.10499,9.30391 " +
                "C7.0337,9.38757 6.9526,9.46229 6.86346,9.52646 L7.73987,10.7438 Z " +

                "M15.386,15.0125 C14.697,15.2714 14.1716,15.4571 13.76,15.8135 L14.742,16.9475 " +
                "C14.9028,16.8082 15.1192,16.7152 15.9136,16.4167 L15.386,15.0125 Z " +

                "M14.1037,18.4001 C14.329,17.5813 14.4021,17.3569 14.5263,17.1838 L13.3075,16.3094 " +
                "C12.9902,16.7517 12.8529,17.2919 12.6574,18.0022 L14.1037,18.4001 Z " +

                "M13.76,15.8135 C13.5903,15.9605 13.4384,16.1269 13.3075,16.3094 L14.5263,17.1838 " +
                "C14.5887,17.0968 14.6611,17.0175 14.742,16.9475 L13.76,15.8135 Z " +

                "M5.15229,14.7781 C4.50615,14.1313 4.06799,13.691 3.78366,13.3338 " +
                "C3.49835,12.9753 3.46889,12.8201 3.46845,12.7505 L1.96848,12.76 " +
                "C1.97215,13.3422 2.26127,13.8297 2.61002,14.2679 " +
                "C2.95976,14.7073 3.47115,15.2176 4.09109,15.8382 L5.15229,14.7781 Z " +

                "M5.6457,9.95044 C4.80048,10.1835 4.10396,10.3743 3.58296,10.5835 " +
                "C3.06341,10.792 2.57116,11.0732 2.28053,11.5778 L3.58038,12.3264 " +
                "C3.615,12.2663 3.71693,12.146 4.1418,11.9755 " +
                "C4.56523,11.8055 5.16337,11.6394 6.04438,11.3965 L5.6457,9.95044 Z " +

                "M3.46845,12.7505 C3.46751,12.6016 3.50616,12.4553 3.58038,12.3264 L2.28053,11.5778 " +
                "C2.07354,11.9372 1.96586,12.3452 1.96848,12.76 L3.46845,12.7505 Z " +

                "M8.20777,19.9591 C8.83164,20.5836 9.34464,21.0987 9.78647,21.4506 " +
                "C10.227,21.8015 10.7179,22.0922 11.3041,22.0931 L11.3064,20.5931 " +
                "C11.2369,20.593 11.0814,20.5644 10.721,20.2773 " +
                "C10.3618,19.9912 9.91923,19.5499 9.26897,18.8989 L8.20777,19.9591 Z " +

                "M12.6574,18.0022 C12.4133,18.8897 12.2462,19.4924 12.0751,19.9188 " +
                "C11.9033,20.3467 11.7821,20.4487 11.7215,20.4833 L12.465,21.7861 " +
                "C12.974,21.4956 13.2573,21.0004 13.4671,20.4775 " +
                "C13.6776,19.9532 13.8694,19.2516 14.1037,18.4001 L12.6574,18.0022 Z " +

                "M11.3041,22.0931 C11.7112,22.0937 12.1114,21.9879 12.465,21.7861 L11.7215,20.4833 " +
                "C11.595,20.5555 11.4519,20.5933 11.3064,20.5931 L11.3041,22.0931 Z " +

                "M18.541,8.56568 C19.6045,9.63022 20.3403,10.3695 20.7917,10.9788 " +
                "C21.2353,11.5774 21.2863,11.8959 21.2321,12.1464 L22.6982,12.4634 " +
                "C22.8881,11.5854 22.5382,10.8162 21.9969,10.0857 " +
                "C21.4635,9.36592 20.6305,8.53486 19.6022,7.50556 L18.541,8.56568 Z " +

                "M17.8307,15.6964 C19.1921,15.1849 20.294,14.773 21.0771,14.3384 " +
                "C21.8718,13.8973 22.5083,13.3416 22.6982,12.4634 L21.2321,12.1464 " +
                "C21.178,12.3968 21.0001,12.6655 20.3491,13.0268 " +
                "C19.6865,13.3946 18.7112,13.7632 17.3032,14.2922 L17.8307,15.6964 Z " +

                "M16.52,4.42014 C15.4841,3.3832 14.6481,2.54353 13.9246,2.00638 " +
                "C13.1908,1.46165 12.4175,1.10912 11.5357,1.30367 L11.8588,2.76845 " +
                "C12.1086,2.71335 12.4277,2.7633 13.0304,3.21075 " +
                "C13.6433,3.66579 14.3876,4.40801 15.4588,5.48026 L16.52,4.42014 Z " +

                "M9.73087,6.74438 C10.2525,5.32075 10.6161,4.33403 10.9812,3.66315 " +
                "C11.3402,3.00338 11.609,2.82357 11.8588,2.76845 L11.5357,1.30367 " +
                "C10.654,1.49819 10.1005,2.14332 9.66362,2.94618 " +
                "C9.23278,3.73793 8.82688,4.85154 8.32243,6.22834 L9.73087,6.74438 Z");

            drawingGroup.Children.Add(new GeometryDrawing(brush, null, geometry));

            var drawingImage = new DrawingImage(drawingGroup);
            drawingImage.Freeze();
            return drawingImage;
        }

        public static DrawingImage ArrowLeft(Color color)
        {
            var drawingGroup = new DrawingGroup();
            var brush = new SolidColorBrush(color);

            var geometry = Geometry.Parse(
                "M10.28,7.22 a0.75,0.75 0 0 1 0,1.06 L7.09,11.5 H18 a0.75,0.75 0 0 1 0,1.5 H7.09 l3.19,3.22 a0.75,0.75 0 1 1 -1.06,1.06 l-4.5,-4.5 a0.75,0.75 0 0 1 0,-1.06 l4.5,-4.5 a0.75,0.75 0 0 1 1.06,0 Z");

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
                "M7.22,10.28 a0.75,0.75 0 0 0 1.06,0 L11.5,7.09 V18 a0.75,0.75 0 0 0 1.5,0 V7.09 l3.22,3.19 a0.75,0.75 0 1 0 1.06,-1.06 l-4.5,-4.5 a0.75,0.75 0 0 0 -1.06,0 l-4.5,4.5 a0.75,0.75 0 0 0 0,1.06 Z");

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

        public static DrawingImage Close(Color color)
        {
            var drawingGroup = new DrawingGroup();

            var geometry = Geometry.Parse("M18,18 L12,12 M12,12 L6,6 M12,12 L18,6 M12,12 L6,18");

            var pen = new Pen(new SolidColorBrush(color), 2)
            {
                StartLineCap = PenLineCap.Round,
                EndLineCap = PenLineCap.Round,
                LineJoin = PenLineJoin.Round
            };

            var drawing = new GeometryDrawing(null, pen, geometry);
            drawingGroup.Children.Add(drawing);

            var drawingImage = new DrawingImage(drawingGroup);
            drawingImage.Freeze();

            return drawingImage;
        }

        public static DrawingImage ClipboardOutlined(Color color)
        {
            var drawingGroup = new DrawingGroup();

            var pen = new Pen(new SolidColorBrush(color), 1.5);
            pen.StartLineCap = PenLineCap.Round;
            pen.EndLineCap = PenLineCap.Round;
            pen.LineJoin = PenLineJoin.Round;
            pen.Freeze();

            var geometry1 = Geometry.Parse(
                "M18.63 7.1499C18.67 7.7599 18.62 8.4499 18.5 9.2199L17.77 13.9099C17.15 17.8199 15.34 19.1399 11.43 18.5299L6.73999 17.7899C5.38999 17.5799 4.34999 17.2199 3.58999 16.6799C2.13999 15.6699 1.71999 14.0099 2.11999 11.4499L2.85999 6.7599C3.47999 2.8499 5.28999 1.5299 9.19999 2.1399L13.89 2.8799C17.03 3.3699 18.5 4.6499 18.63 7.1499Z");
            drawingGroup.Children.Add(new GeometryDrawing(null, pen, geometry1));

            var geometry2 = Geometry.Parse(
                "M20.5 13.4699L19 17.9799C17.75 21.7399 15.75 22.7399 11.99 21.4899L7.48003 19.9899C5.21003 19.2399 3.95003 18.1999 3.59003 16.6799C4.35003 17.2199 5.39003 17.5799 6.74003 17.7899L11.43 18.5299C15.34 19.1399 17.15 17.8199 17.77 13.9099L18.5 9.2199C18.62 8.4499 18.67 7.7599 18.63 7.1499C21.02 8.4199 21.54 10.3399 20.5 13.4699Z");
            drawingGroup.Children.Add(new GeometryDrawing(null, pen, geometry2));

            var geometry3 = Geometry.Parse(
                "M8.24 8.98C9.20098 8.98 9.98 8.20098 9.98 7.24C9.98 6.27902 9.20098 5.5 8.24 5.5C7.27902 5.5 6.5 6.27902 6.5 7.24C6.5 8.20098 7.27902 8.98 8.24 8.98Z");
            drawingGroup.Children.Add(new GeometryDrawing(null, pen, geometry3));

            var drawingImage = new DrawingImage(drawingGroup);
            drawingImage.Freeze();
            return drawingImage;
        }

        public static DrawingImage ClipboardFilled(Color color)
        {
            var drawingGroup = new DrawingGroup();

            var brush = new SolidColorBrush(color);
            brush.Freeze();

            var geometry1 = Geometry.Parse(
                "M13.89 2.87819L9.19999 2.13819C5.28999 1.52819 3.47999 2.84819 2.85999 6.75819L2.11999 11.4482C1.71999 14.0082 2.13999 15.6682 3.58999 16.6782C4.34999 17.2182 5.38999 17.5782 6.73999 17.7882L11.43 18.5282C15.34 19.1382 17.15 17.8182 17.77 13.9082L18.5 9.21819C18.62 8.44819 18.67 7.75819 18.63 7.14819C18.5 4.64819 17.03 3.36819 13.89 2.87819ZM8.23999 9.34819C7.06999 9.34819 6.11999 8.39819 6.11999 7.23819C6.11999 6.06819 7.06999 5.11819 8.23999 5.11819C9.39999 5.11819 10.35 6.06819 10.35 7.23819C10.35 8.39819 9.39999 9.34819 8.23999 9.34819Z");
            drawingGroup.Children.Add(new GeometryDrawing(brush, null, geometry1));

            var geometry2 = Geometry.Parse(
                "M20.5006 13.4686L19.0006 17.9786C17.7506 21.7386 15.7506 22.7386 11.9906 21.4886L7.48063 19.9886C6.07063 19.5186 5.05063 18.9386 4.39062 18.2086C5.02062 18.4586 5.75063 18.6486 6.58063 18.7786L11.2806 19.5186C11.9206 19.6186 12.5206 19.6686 13.0806 19.6686C16.3806 19.6686 18.1506 17.8886 18.7606 14.0586L19.4906 9.36859C19.5906 8.78859 19.6306 8.27859 19.6306 7.80859C21.1506 9.05859 21.3706 10.8386 20.5006 13.4686Z");
            drawingGroup.Children.Add(new GeometryDrawing(brush, null, geometry2));

            var drawingImage = new DrawingImage(drawingGroup);
            drawingImage.Freeze();
            return drawingImage;
        }
    }
}
