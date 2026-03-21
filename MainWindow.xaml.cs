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
    private bool _convertingAutomatically = false;
    private bool _autoConversionAllowed = true;
    private string _cancelledClipboardSuggestionContent = string.Empty;
    private string _lastCachedClipboardContent = string.Empty;
    private string _detectedCborInClipboard = string.Empty;
    private string _cancelledDetectedCborInClipboard = string.Empty;
    private readonly DispatcherTimer _typingTimer;
    private readonly DispatcherTimer _convertingTimer;
    private readonly Lock _conversionTaskLock = new();

    public MainWindow()
    {
        InitializeComponent();
        InitializeGuiElements();

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
            StartTheConversionTask(InputTextBox.Text);
            _autoConversionAllowed = true;
        });
    }

    private void InitialzeAutoConvertInputTypeComboBox()
    {
        AutoConvertInputTypeComboBox.ItemsSource = new List<string>()
        {
            "None",
            ConvertingTypes.Text.ToString(),
            ConvertingTypes.Hex.ToString(),
            ConvertingTypes.Base64.ToString(),
            ConvertingTypes.Base64URL.ToString(),
            "CBOR (Exact)",
            "CBOR (In content)",
        };
        AutoConvertInputTypeComboBox.SelectedIndex = 0;
    }

    private void InitializeGuiElements()
    {
        InitialzeAutoConvertInputTypeComboBox();
        PanelChangeIcon.Source = CustomIcons.ScreenVertical(SystemColors.AccentColor);
        CopyButtonIcon.Source = CustomIcons.Copy(SystemColors.AccentColor);
        ReverseButtonIcon.Source = CustomIcons.ArrowUp(SystemColors.AccentColor);
        ClipboardSuggestionCancelButtonIcon.Source = CustomIcons.Close(Colors.White);
        DetectedCborDecodeSuggestionCancelButtonIcon.Source = CustomIcons.Close(Colors.White);
        DetectedCborDecodeSuggestionButton.Background = SystemColors.AccentColorBrush;
        TextEncodingDecodingCombobox.ItemsSource = Utils.GetEncodingDecodingTypes();
        AutoConvertTextEncodingDecodingCombobox.ItemsSource = Utils.GetEncodingDecodingTypes();
        TextEncodingDecodingCombobox.SelectedIndex = 0;
        AutoConvertTextEncodingDecodingCombobox.SelectedIndex = 0;
    }

    private void ShowDefaultMessage()
    {
        Dispatcher.Invoke(() =>
        {
            ShowMessage("Convert as you wish ;)");
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
            ShowDefaultMessage();
        });
    }

    private void ShowMessage(string content, int duration = -1)
    {
        ErrorTextBox.Text = null;
        MessageTextBox.Text = content;
        ErrorTextBox.Visibility = Visibility.Collapsed;
        MessageTextBox.Visibility = Visibility.Visible;
        if (duration > 0)
        {
            Task.Run(() =>
            {
                Task.Delay(duration).Wait();
                ShowDefaultMessage();
            });
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

                    if (!_convertingAutomatically)
                    {
                        InputComboBox.ItemsSource = Utils.GetInputConvertingTypes(input);
                        InputComboBox.SelectedIndex = 0;

                        OutputComboBox.ItemsSource = Utils.GetOutputConvertingTypes(input, (ConvertingTypes)InputComboBox.SelectedItem);
                        OutputComboBox.SelectedIndex = 0;
                    }
                    else
                    {
                        _convertingAutomatically = false;
                    }

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

    private bool TryAutoConversion(string input)
    {
        try
        {
            if (!_autoConversionAllowed)
            {
                return false;
            }

            if (AutoConvertInputTypeComboBox.SelectedItem.ToString() == "None")
            {
                return false;
            }

            ConvertingTypes autoConvertInputType;
            bool cborInContent = false;
            if (AutoConvertInputTypeComboBox.SelectedItem.ToString()?.Contains("CBOR") == true)
            {
                autoConvertInputType = ConvertingTypes.CBOR;
                cborInContent = AutoConvertInputTypeComboBox.SelectedItem?.ToString()?.Contains("content") == true;
            }
            else if (Enum.TryParse(typeof(ConvertingTypes), AutoConvertInputTypeComboBox.SelectedItem.ToString(), out object? selectedAutoConvertTypeObject))
            {
                autoConvertInputType = (ConvertingTypes)selectedAutoConvertTypeObject;
            }
            else
            {
                return false;
            }

            if (cborInContent)
            {
                if (string.IsNullOrEmpty(_detectedCborInClipboard))
                {
                    return false;
                }
                input = _detectedCborInClipboard;
            }

            var inputTypes = Utils.GetInputConvertingTypes(input);

            if (!inputTypes.Contains(autoConvertInputType))
            {
                return false;
            }

            _convertOnTypeChange = false;
            InputComboBox.ItemsSource = inputTypes;
            InputComboBox.SelectedItem = autoConvertInputType;

            var outputTypes = Utils.GetOutputConvertingTypes(input, autoConvertInputType);

            if (!outputTypes.Contains((ConvertingTypes)AutoConvertOutputTypeComboBox.SelectedItem))
            {
                return false;
            }

            OutputComboBox.ItemsSource = outputTypes;
            OutputComboBox.SelectedItem = (ConvertingTypes)AutoConvertOutputTypeComboBox.SelectedItem;
            _convertOnTypeChange = true;

            _convertingAutomatically = true;
            InputTextBox.Text = input;
            return true;
        }
        catch
        {
            return false;
        }
    }

    private ConvertingTypes? GetAutoConvertingInputType()
    {
        if (AutoConvertInputTypeComboBox.SelectedItem.ToString()?.Contains("CBOR") == true)
        {
            return ConvertingTypes.CBOR;
        }
        else if (Enum.TryParse(typeof(ConvertingTypes), AutoConvertInputTypeComboBox.SelectedItem.ToString(), out object? selectedAutoConvertTypeObject))
        {
            return (ConvertingTypes)selectedAutoConvertTypeObject;
        }

        return null;
    }

    private void SetupAutoConvertEncodingDecodingComboBoxVisibility(ConvertingTypes inputType)
    {
        Task.Run(() =>
        {
            Task.Delay(10).Wait();
            Dispatcher.Invoke(() =>
            {
                if ((inputType == ConvertingTypes.Text && (ConvertingTypes)AutoConvertOutputTypeComboBox.SelectedItem != ConvertingTypes.CBOR) ||
                    ((ConvertingTypes)AutoConvertOutputTypeComboBox.SelectedItem == ConvertingTypes.Text && inputType != ConvertingTypes.CBOR))
                {
                    AutoConvertTextEncodingDecodingCombobox.Visibility = Visibility.Visible;
                }
                else
                {
                    AutoConvertTextEncodingDecodingCombobox.Visibility = Visibility.Collapsed;
                }
            });
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
        InputHint.Visibility = string.IsNullOrEmpty(InputTextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
        DetectedCborDecodeSuggestionButtonsGrid.Visibility = Visibility.Collapsed;
        ClipboardSuggestionButtonsGrid.Visibility = Visibility.Collapsed;
        _autoConversionAllowed = false;

        _typingTimer.Stop();
        _convertingTimer.Stop();

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
                _lastCachedClipboardContent = Clipboard.GetText();
                Task.Run(() =>
                {
                    var inputTypes = Utils.GetInputConvertingTypes(_lastCachedClipboardContent);

                    _detectedCborInClipboard = string.Empty;
                    if (!inputTypes.Contains(ConvertingTypes.CBOR) && _lastCachedClipboardContent.ContainsCbor(out string detectedCborHex) &&
                        detectedCborHex != _detectedCborInClipboard)
                    {
                        _detectedCborInClipboard = detectedCborHex;
                    }

                    if (!string.IsNullOrEmpty(_lastCachedClipboardContent) && inputTypes.Count > 0)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            if (!TryAutoConversion(_lastCachedClipboardContent))
                            {
                                if (_lastCachedClipboardContent != _cancelledClipboardSuggestionContent)
                                {
                                    _cancelledClipboardSuggestionContent = string.Empty;
                                }

                                if (_detectedCborInClipboard != _cancelledDetectedCborInClipboard)
                                {
                                    _cancelledDetectedCborInClipboard = string.Empty;
                                }

                                if (InputTextBox.Text != _lastCachedClipboardContent &&
                                    _lastCachedClipboardContent != _cancelledClipboardSuggestionContent)
                                {
                                    ClipboardSuggestionButtonsGrid.Visibility = Visibility.Visible;
                                }

                                if (InputTextBox.Text != _detectedCborInClipboard && 
                                    _detectedCborInClipboard != _cancelledDetectedCborInClipboard)
                                {
                                    DetectedCborDecodeSuggestionButtonsGrid.Visibility = Visibility.Visible;
                                }
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
        if (!string.IsNullOrEmpty(_lastCachedClipboardContent))
        {
            _convertImmediately = true;
            InputTextBox.Text = _lastCachedClipboardContent;
        }
    }

    private void ClipboardSuggestionCancelClick(object sender, RoutedEventArgs e)
    {
        ClipboardSuggestionButtonsGrid.Visibility = Visibility.Collapsed;
        _cancelledClipboardSuggestionContent = _lastCachedClipboardContent;
    }

    private void DetectedCborDecodeSuggestionCancelButtonClick(object sender, RoutedEventArgs e)
    {
        DetectedCborDecodeSuggestionButtonsGrid.Visibility = Visibility.Collapsed;
        _cancelledDetectedCborInClipboard = _detectedCborInClipboard;
    }

    private void DetectedCborDecodeSuggestionButtonClick(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(_detectedCborInClipboard))
        {
            _convertImmediately = true;
            InputTextBox.Text = _detectedCborInClipboard;
        }
    }

    private void AutoConvertInputTypeComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var inputType = GetAutoConvertingInputType();

        if (inputType is null)
        {
            AutoConvertOutputElementsStack.Visibility = Visibility.Collapsed;
            return;
        }

        AutoConvertOutputElementsStack.Visibility = Visibility.Visible;
        var outputTypes = Utils.GetOutputConvertingTypes(null, inputType.Value);
        AutoConvertOutputTypeComboBox.ItemsSource = outputTypes;
        AutoConvertOutputTypeComboBox.SelectedIndex = 0;
        SetupAutoConvertEncodingDecodingComboBoxVisibility(inputType.Value);
    }

    private void AutoConvertOutputTypeComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var inputType = GetAutoConvertingInputType();
        if (inputType is null)
        {
            return;
        }
        SetupAutoConvertEncodingDecodingComboBoxVisibility(inputType.Value);
    }
    #endregion
}
