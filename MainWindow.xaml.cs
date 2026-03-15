using Convertie.Assets;
using Convertie.Utilities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

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
    private bool _convertImmediately = false;
    private bool _clipboardSuggestionCancelled = false;
    private string _cancelledClipboardSuggestionContent = string.Empty;
    private readonly DispatcherTimer _typingTimer;
    private readonly DispatcherTimer _convertingTimer;
    private readonly Lock _conversionTaskLock = new();

    public MainWindow()
    {
        InitializeComponent();
        PanelChangeIcon.Source = CustomIcons.ScreenVertical(SystemColors.AccentColor);
        CopyButtonIcon.Source = CustomIcons.Copy(SystemColors.AccentColor);
        ReverseButtonIcon.Source = CustomIcons.ArrowUp(SystemColors.AccentColor);
        ClipboardSuggestionCancelButtonIcon.Source = CustomIcons.Close(Colors.White);
        ClipboardSuggestionButton.Background = SystemColors.AccentColorBrush;
        TextEncodingDecodingCombobox.ItemsSource = Utils.GetEncodingDecodingTypes();
        TextEncodingDecodingCombobox.SelectedIndex = 0;

        _typingTimer = new();
        _typingTimer.Tick += ConvertionTimerTimeout;
        _typingTimer.Interval = TimeSpan.FromMilliseconds(500);
        _convertingTimer = new();
        _convertingTimer.Tick += ConvertionTimerTimeout;
        _convertingTimer.Interval = TimeSpan.FromMilliseconds(50);
    }

    private void ConvertionTimerTimeout(object? sender, EventArgs e)
    {
        _typingTimer.Stop();
        _convertingTimer.Stop();
        Dispatcher.Invoke(() =>
        {
            ClipboardSuggestionButtonsGrid.Visibility = Visibility.Collapsed;
            StartTheConversionTask(InputTextBox.Text);
        });
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

    private void StartTheConversionTask(string input)
    {
        Task.Run(() =>
        {
            lock (_conversionTaskLock)
            {
                Dispatcher.Invoke(() =>
                {
                    if (string.IsNullOrEmpty(input))
                    {
                        OutputElementsGrid.Visibility = Visibility.Collapsed;
                        InputElementTitle.Visibility = Visibility.Collapsed;
                        InputComboBox.Visibility = Visibility.Collapsed;
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
                    InputElementTitle.Visibility = Visibility.Visible;
                    InputComboBox.Visibility = Visibility.Visible;

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
        InputHint.Visibility = Visibility.Collapsed;
        _typingTimer.Stop();
        _convertingTimer.Stop();

        if (string.IsNullOrEmpty(InputTextBox.Text))
        {
            InputHint.Visibility = Visibility.Visible;
        }

        if (_convertImmediately || string.IsNullOrEmpty(InputTextBox.Text))
        {
            _convertingTimer.Start();
        }
        else
        {
            _typingTimer.Start();
        }

        _convertImmediately = false;
    }

    private void InputTextBoxGotFocus(object sender, RoutedEventArgs e)
    {
        Task.Run(() =>
        {
            Task.Delay(20).Wait();
            Dispatcher.Invoke(() =>
            {
                var clipboardText = Clipboard.GetText();
                Task.Run(() =>
                {
                    if (!string.IsNullOrEmpty(clipboardText) && Utils.GetInputConvertingTypes(clipboardText).Count > 0)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            if (clipboardText != _cancelledClipboardSuggestionContent)
                            {
                                _clipboardSuggestionCancelled = false;
                                _cancelledClipboardSuggestionContent = string.Empty;
                            }
                            if (!_clipboardSuggestionCancelled && InputTextBox.Text != clipboardText)
                            {
                                ClipboardSuggestionButtonsGrid.Visibility = Visibility.Visible;
                            }
                        });
                    }
                });
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
        _convertImmediately = true;
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
            _convertImmediately = true;
            InputTextBox.Text = clipboardText;
        }
    }

    private void ClipboardSuggestionCancelClick(object sender, RoutedEventArgs e)
    {
        ClipboardSuggestionButtonsGrid.Visibility = Visibility.Collapsed;
        _clipboardSuggestionCancelled = true;
        _cancelledClipboardSuggestionContent = Clipboard.GetText();
        InputTextBox.Focus();
    }
    #endregion
}
