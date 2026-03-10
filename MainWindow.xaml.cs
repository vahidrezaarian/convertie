using Convertie.Assets;
using Convertie.Utilities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Convertie;

public enum ConvertingTypes
{
    Text,
    Hex,
    Base64,
    Base64URL,
    CBOR
}

public enum EncodingTypes
{
    UTF8,
    ASCII,
    UTF32,
    Unicode,
    BigEndianUnicode,
    Latin1
}

public partial class MainWindow : Window
{
    private bool _isHorizontal = true;
    private bool _convertOnTypeChange = false;
    private readonly Lock _conversionTaskLock = new();

    public MainWindow()
    {
        InitializeComponent();
        PanelChangeIcon.Source = CustomIcons.ScreenVertical(SystemColors.AccentColor);
        CopyButtonIcon.Source = CustomIcons.Copy(SystemColors.AccentColor);
        ReverseButtonIcon.Source = CustomIcons.ArrowUp(SystemColors.AccentColor);
        ClipboardSuggestionButton.Background = SystemColors.AccentColorBrush;
        TextEncodingDecodingCombobox.ItemsSource = Utils.GetEncodingDecodingTypes();
        TextEncodingDecodingCombobox.SelectedIndex = 0;
    }

    private void ShowError(string content, int duration = 3000)
    {
        ErrorTextBox.Text = content;
        MessageTextBox.Visibility = Visibility.Collapsed;
        ErrorTextBox.Visibility = Visibility.Visible;
        Task.Run(() =>
        {
            Task.Delay(duration).Wait();
            Dispatcher.Invoke(() =>
            {
                ShowMessage("Convert as you wish ;)");
            });
        });
    }

    private void ShowMessage(string content)
    {
        ErrorTextBox.Text = null;
        MessageTextBox.Text = content;
        ErrorTextBox.Visibility = Visibility.Collapsed;
        MessageTextBox.Visibility = Visibility.Visible;
    }

    private void Convert()
    {
        try
        {
            OutputTextBox.Text = InputTextBox.Text.Convert((ConvertingTypes)InputComboBox.SelectedItem,
                (ConvertingTypes)OutputComboBox.SelectedItem,
                (EncodingTypes)TextEncodingDecodingCombobox.SelectedItem);
            SetupContentGrid();
        }
        catch (Exception ex)
        {
            ShowError($"Failed to convert. Error: {ex.Message}", 5000);
        }
    }

    private void CreateHorizontalPanel()
    {
        ContentGrid.ColumnDefinitions.Clear();
        ContentGrid.RowDefinitions.Clear();

        ContentGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        if (OutputElementsGrid.IsVisible)
        {
            ContentGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        }
        else
        {
            ContentGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
        }
        OutputElementsGrid.Margin = new Thickness(0, 10, 0, 0);

        Grid.SetRow(InputElementsGrid, 0);
        Grid.SetRow(OutputElementsGrid, 1);

        ReverseButtonIcon.Source = CustomIcons.ArrowUp(SystemColors.AccentColor);
        PanelChangeIcon.Source = CustomIcons.ScreenVertical(SystemColors.AccentColor);
    }

    private void CreateVerticalPanel()
    {
        ContentGrid.ColumnDefinitions.Clear();
        ContentGrid.RowDefinitions.Clear();

        ContentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        if (OutputElementsGrid.IsVisible)
        {
            ContentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        }
        else
        {
            ContentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
        }
        OutputElementsGrid.Margin = new Thickness(10, 0, 0, 0);

        Grid.SetColumn(InputElementsGrid, 0);
        Grid.SetColumn(OutputElementsGrid, 1);

        ReverseButtonIcon.Source = CustomIcons.ArrowLeft(SystemColors.AccentColor);
        PanelChangeIcon.Source = CustomIcons.ScreenHorizontal(SystemColors.AccentColor);
    }

    private void SetupContentGrid(bool togglePanelDirection = false)
    {
        if (_isHorizontal)
        {
            if (togglePanelDirection)
            {
                _isHorizontal = false;
                CreateVerticalPanel();
            }
            else
            {
                CreateHorizontalPanel();
            }
        }
        else
        {
            if (togglePanelDirection)
            {
                _isHorizontal = true;
                CreateHorizontalPanel();
            }
            else
            {
                CreateVerticalPanel();
            }
        }
    }

    private void SetEncodingDecodingComboBox()
    {
        TextEncodingDecodingCombobox.Visibility = Visibility.Collapsed;
        if (InputComboBox.SelectedItem != null && (ConvertingTypes)InputComboBox.SelectedItem != ConvertingTypes.CBOR &&
            OutputComboBox.SelectedItem != null && (ConvertingTypes)OutputComboBox.SelectedItem != ConvertingTypes.CBOR)
        {
            if ((ConvertingTypes)InputComboBox.SelectedItem == ConvertingTypes.Text || (ConvertingTypes)OutputComboBox.SelectedItem == ConvertingTypes.Text)
            {
                TextEncodingDecodingCombobox.Visibility = Visibility.Visible;
            }
        }
    }

    private void CreateConversionTask(string input)
    {
        Task.Run(() =>
        {
            lock (_conversionTaskLock)
            {
                Dispatcher.Invoke(() =>
                {
                    OutputElementsGrid.Visibility = Visibility.Collapsed;
                    InputElementTitle.Visibility = Visibility.Collapsed;
                    InputComboBox.Visibility = Visibility.Collapsed;

                    if (string.IsNullOrEmpty(input))
                    {
                        SetupContentGrid();
                        return;
                    }

                    _convertOnTypeChange = false;

                    InputComboBox.ItemsSource = Utils.GetInputConvertingTypes(input);
                    InputComboBox.SelectedIndex = 0;

                    OutputComboBox.ItemsSource = Utils.GetOutputConvertingTypes(input, (ConvertingTypes)InputComboBox.SelectedItem);
                    OutputComboBox.SelectedIndex = 0;

                    SetEncodingDecodingComboBox();

                    OutputElementsGrid.Visibility = Visibility.Visible;
                    if (InputComboBox.Items.Count > 1)
                    {
                        InputElementTitle.Visibility = Visibility.Visible;
                        InputComboBox.Visibility = Visibility.Visible;
                    }

                    Convert();
                    _convertOnTypeChange = true;
                });
            }
        });
    }

    #region Overrides
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        DragMove();
    }
    #endregion

    #region Callbacks
    private void InputTextBoxTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        ClipboardSuggestionButton.Visibility = Visibility.Collapsed;
        CreateConversionTask(InputTextBox.Text);
    }

    private void InputTextBoxGotFocus(object sender, RoutedEventArgs e)
    {
        var clipboardText = Clipboard.GetText();
        InputHint.Visibility = Visibility.Collapsed;
        if (!string.IsNullOrEmpty(clipboardText) && InputTextBox.Text != clipboardText)
        {
            if (Utils.GetInputConvertingTypes(clipboardText).Count > 0)
            {
                ClipboardSuggestionButton.Visibility = Visibility.Visible;
            }
        }
    }

    private void InputTextBoxLostFocus(object sender, RoutedEventArgs e)
    {
        Task.Run(() =>
        {
            Task.Delay(100).Wait();
            Dispatcher.Invoke(() =>
            {
                if (string.IsNullOrEmpty(InputTextBox.Text))
                {
                    InputHint.Visibility = Visibility.Visible;
                }
            });
        });
    }

    private void ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_convertOnTypeChange)
        {
            if (e.Source is ComboBox comboBox && comboBox.Name == "InputComboBox")
            {
                OutputComboBox.ItemsSource = Utils.GetOutputConvertingTypes(InputTextBox.Text, (ConvertingTypes)InputComboBox.SelectedItem);
                OutputComboBox.SelectedIndex = 0;
            }

            SetEncodingDecodingComboBox();

            Convert();
        }
    }

    private void PanelViewChangeButtonClick(object sender, RoutedEventArgs e)
    {
        SetupContentGrid(true);
    }

    private void ReverseButtonClick(object sender, RoutedEventArgs e)
    {
        InputTextBox.Text = OutputTextBox.Text;
    }

    private void CopyButtonClick(object sender, RoutedEventArgs e)
    {
        CopyButtonIcon.Source = CustomIcons.Copied(Colors.LimeGreen);
        Clipboard.SetText(OutputTextBox.Text);
        Task.Run(() =>
        {
            Task.Delay(2000).Wait();
            Dispatcher.Invoke(() =>
            {
                CopyButtonIcon.Source = CustomIcons.Copy(SystemColors.AccentColor);
            });
        });
    }

    private void ClipboardSuggestionButtonClick(object sender, RoutedEventArgs e)
    {
        var clipboardText = Clipboard.GetText();
        if (!string.IsNullOrEmpty(clipboardText))
        {
            InputTextBox.Text = clipboardText;
        }
    }
    #endregion
}
