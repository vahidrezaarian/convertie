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
    Binary,
    Decimal,
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
    private bool _convertOnTextChange = false;
    private bool _convertImmediately = false;
    private ConvertingTypes? _manuallySelectedInputType = null;
    private ConvertingTypes? _manuallySelectedOutputType = null;
    private bool _initialAutoConvertingSelection = true;
    private string _cancelledClipboardSuggestionContent = string.Empty;
    private string _lastCachedClipboardContent = string.Empty;
    private string _detectedCborInClipboard = string.Empty;
    private string _cancelledDetectedCborInClipboard = string.Empty;
    private string _lastAutoConvertedInput = string.Empty;
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
        _convertingTimer.Interval = TimeSpan.FromMilliseconds(10);
    }

    private void ConvertionTimerTimeout(object? sender, EventArgs e)
    {
        _typingTimer.Stop();
        _convertingTimer.Stop();
        Dispatcher.Invoke(() =>
        {
            StartTheConversionTask(InputTextBox.Text);
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
            ConvertingTypes.Binary.ToString(),
            ConvertingTypes.Decimal.ToString(),
            "CBOR (Exact)",
            "CBOR (In content)",
        };
        AutoConvertInputTypeComboBox.SelectedIndex = Properties.Settings.Default.AutoConvertingInput;
        var inputType = GetAutoConvertingInputType();
        if (inputType != null )
        {
            AutoConvertInputComboboxTitle.Visibility = Visibility.Visible;
            SetupAutoConvertingOutputComboBox(inputType.Value, Properties.Settings.Default.AutoConvertingOutput);
            SetupAutoConvertEncodingDecodingComboBoxVisibility(inputType.Value, Properties.Settings.Default.AutoConvertingEncodingDecoding);
        }
    }

    private void InitializeGuiElements()
    {
        InitialzeAutoConvertInputTypeComboBox();
        SetupTopMostButtonAndValue();
        PanelChangeIcon.Source = CustomIcons.ScreenVertical(SystemColors.AccentColor);
        CopyButtonIcon.Source = CustomIcons.Copy(SystemColors.AccentColor);
        ReverseButtonIcon.Source = CustomIcons.ArrowUp(SystemColors.AccentColor);
        ClipboardSuggestionCancelButtonIcon.Source = CustomIcons.Close(Colors.White);
        DetectedCborDecodeSuggestionCancelButtonIcon.Source = CustomIcons.Close(Colors.White);
        ClipboardSuggestionButton.Background = SystemColors.AccentColorBrush;
        ClipboardSuggestionButton.Foreground = Brushes.White;
        DetectedCborDecodeSuggestionButton.Background = SystemColors.AccentColorBrush;
        DetectedCborDecodeSuggestionButton.Foreground = Brushes.White;
        ClipboardSuggestionCancelButton.Background = SystemColors.AccentColorBrush;
        SuggestionGridBackground.Background = SystemColors.GradientActiveCaptionBrush;
        DetectedCborDecodeSuggestionCancelButton.Background = SystemColors.AccentColorBrush;
        TextEncodingDecodingCombobox.ItemsSource = Utils.GetEncodingDecodingTypes();
        AutoConvertTextEncodingDecodingCombobox.ItemsSource = Utils.GetEncodingDecodingTypes();
        TextEncodingDecodingCombobox.SelectedIndex = 0;
        AutoConvertTextEncodingDecodingCombobox.SelectedIndex = 0;

        if (Properties.Settings.Default.PanelOrientation == 1)
        {
            SetupContentGrid(true);
        }

        Loaded += (s, e) =>
        {
            Task.Run(() =>
            {
                Task.Delay(100).Wait();
                _convertOnTextChange = true;
                _convertOnTypeChange = true;
            });
        };
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
                Properties.Settings.Default.PanelOrientation = 1;
                Properties.Settings.Default.Save();
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
                Properties.Settings.Default.PanelOrientation = 0;
                Properties.Settings.Default.Save();
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
            UpdateInputLength();
            UpdateOutputLength();
        }
        catch (Exception ex)
        {
            ShowError($"Failed to convert. Error: {ex.Message}", 5000);
            OutputTextBox.Text = null;
        }
    }

    private void SetEncodingDecodingComboBoxVisibility()
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

    private void SetupAutoConvertEncodingDecodingComboBoxVisibility(ConvertingTypes inputType, int selectedIndex)
    {
        Task.Run(() =>
        {
            Task.Delay(10).Wait();
            Dispatcher.Invoke(() =>
            {
                AutoConvertTextEncodingDecodingCombobox.Visibility = Visibility.Collapsed;
                if ((inputType == ConvertingTypes.Text && AutoConvertOutputTypeComboBox.SelectedItem != null && (ConvertingTypes)AutoConvertOutputTypeComboBox.SelectedItem != ConvertingTypes.CBOR) ||
                    (AutoConvertOutputTypeComboBox.SelectedItem != null && (ConvertingTypes)AutoConvertOutputTypeComboBox.SelectedItem == ConvertingTypes.Text && inputType != ConvertingTypes.CBOR))
                {
                    AutoConvertTextEncodingDecodingCombobox.Visibility = Visibility.Visible;
                    AutoConvertTextEncodingDecodingCombobox.SelectedIndex = selectedIndex;
                }
            });
        });
    }

    private void SetupAutoConvertingOutputComboBox(ConvertingTypes inputType, int selectedIndex)
    {
        AutoConvertOutputElementsStack.Visibility = Visibility.Visible;
        var outputTypes = Utils.GetOutputConvertingTypes(null, inputType);
        AutoConvertOutputTypeComboBox.ItemsSource = outputTypes;
        AutoConvertOutputTypeComboBox.SelectedIndex = selectedIndex;
    }

    private void SetClipoboardSuggestionGridVisibility(Visibility visibility)
    {
        if (visibility == Visibility.Visible)
        {
            SuggestionButtonsGrid.Visibility = Visibility.Visible;
            ClipboardSuggestionButtonsGrid.Visibility = Visibility.Visible;
        }
        else
        {
            ClipboardSuggestionButtonsGrid.Visibility = Visibility.Collapsed;
            if (DetectedCborDecodeSuggestionButtonsGrid.Visibility == Visibility.Collapsed)
            {
                SuggestionButtonsGrid.Visibility = Visibility.Collapsed;
            }
        }
    }

    private void SetDetectedCborDecodeSuggestionGridVisibility(Visibility visibility)
    {
        if (visibility == Visibility.Visible)
        {
            DetectedCborDecodeSuggestionButtonsGrid.Visibility = Visibility.Visible;
            SuggestionButtonsGrid.Visibility = Visibility.Visible;
        }
        else
        {
            DetectedCborDecodeSuggestionButtonsGrid.Visibility = Visibility.Collapsed;
            if (ClipboardSuggestionButtonsGrid.Visibility == Visibility.Collapsed)
            {
                SuggestionButtonsGrid.Visibility = Visibility.Collapsed;
            }
        }
    }

    private void UpdateInputLength()
    {
        if (InputComboBox.SelectedItem is null)
        {
            InputLength.Visibility = Visibility.Collapsed;
            return;
        }

        if (string.IsNullOrEmpty(InputTextBox.Text))
        {
            return;
        }

        var inputType = (ConvertingTypes)InputComboBox.SelectedItem;
        var input = InputTextBox.Text;

        InputLength.Text = inputType switch
        {
            ConvertingTypes.Hex or ConvertingTypes.CBOR => $"({input.ToByteArrayFromHexString().Length} bytes)  ({input.Length} characters)",
            ConvertingTypes.Binary or ConvertingTypes.CBOR => $"({input.ToByteArrayFromBinaryString().Length} bytes) ({input.Length} bits)",
            ConvertingTypes.Base64 => $"({input.ToByteArrayFromBase64String().Length} bytes)  ({input.Length} characters)",
            ConvertingTypes.Base64URL => $"({input.ToByteArrayFromBase64UrlString().Length} bytes)  ({input.Length} characters)",
            _ => $"({input.Length} characters)",
        };
        InputLength.Visibility = Visibility.Visible;
    }

    private void UpdateOutputLength()
    {
        if (OutputComboBox.SelectedItem is null)
        {
            OutputLength.Visibility = Visibility.Collapsed;
            return;
        }

        if (string.IsNullOrEmpty(InputTextBox.Text))
        {
            return;
        }

        var inputType = (ConvertingTypes)OutputComboBox.SelectedItem;
        var input = OutputTextBox.Text;

        OutputLength.Text = inputType switch
        {
            ConvertingTypes.Hex or ConvertingTypes.CBOR => $"({input.ToByteArrayFromHexString().Length} bytes)  ({input.Length} characters)",
            ConvertingTypes.Binary or ConvertingTypes.CBOR => $"({input.ToByteArrayFromBinaryString().Length} bytes) ({input.Length} bits)",
            ConvertingTypes.Base64 => $"({input.ToByteArrayFromBase64String().Length} bytes)  ({input.Length} characters)",
            ConvertingTypes.Base64URL => $"({input.ToByteArrayFromBase64UrlString().Length} bytes)  ({input.Length} characters)",
            _ => $"({input.Length} characters)",
        };
        OutputLength.Visibility = Visibility.Visible;
    }

    private void ShowControlls(string input)
    {
        OutputElementsGrid.Visibility = Visibility.Visible;
        InputElementTitle.Visibility = Visibility.Visible;
        InputButtonsStack.Visibility = Visibility.Visible;
        RemoveSpacesButton.Visibility = input.ContainsWhiteSpaces() ? Visibility.Visible : Visibility.Collapsed;
    }

    private void HideControls()
    {
        OutputElementsGrid.Visibility = Visibility.Collapsed;
        InputElementTitle.Visibility = Visibility.Collapsed;
        InputButtonsStack.Visibility = Visibility.Collapsed;
        SetupContentGrid();
    }

    private void SetInputAndOutTypes(string input)
    {
        _convertOnTypeChange = false;

        var inputTypes = Utils.GetInputConvertingTypes(input);
        InputComboBox.ItemsSource = inputTypes;
        if (_manuallySelectedInputType != null && inputTypes.Contains(_manuallySelectedInputType.Value))
        {
            InputComboBox.SelectedItem = _manuallySelectedInputType;
        }
        else
        {
            InputComboBox.SelectedIndex = 0;
        }
        var outputTypes = Utils.GetOutputConvertingTypes(input, (ConvertingTypes)InputComboBox.SelectedItem);
        OutputComboBox.ItemsSource = outputTypes;
        if (_manuallySelectedOutputType != null && outputTypes.Contains(_manuallySelectedOutputType.Value))
        {
            OutputComboBox.SelectedItem = _manuallySelectedOutputType;
        }
        else
        {
            OutputComboBox.SelectedIndex = 0;
        }

        _convertOnTypeChange = true;
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
                        HideControls();
                        return;
                    }

                    SetInputAndOutTypes(input);
                    SetEncodingDecodingComboBoxVisibility();
                    ShowControlls(input);
                    Convert();
                    
                });
            }
        });
    }

    private bool TryAutoConversion(string input)
    {
        try
        {
            if (AutoConvertInputTypeComboBox.SelectedItem.ToString() == "None")
            {
                return false;
            }

            if (!string.IsNullOrEmpty(_lastAutoConvertedInput) && (_lastAutoConvertedInput == _lastCachedClipboardContent || _lastAutoConvertedInput == InputTextBox.Text))
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

            _manuallySelectedInputType = autoConvertInputType;
            _manuallySelectedOutputType = (ConvertingTypes)AutoConvertOutputTypeComboBox.SelectedItem;

            InputTextBox.Text = input;
            _lastAutoConvertedInput = input;
            return true;
        }
        catch
        {
            return false;
        }
    }

    private void SetupTopMostButtonAndValue()
    {
        Topmost = Properties.Settings.Default.TopMosetStatus;

        if (Topmost)
        {
            WindowPinToggleButtonIcon.Source = CustomIcons.Pinned(SystemColors.AccentColor);
            WindowPinToggleButton.ToolTip = "Unpin the window from the screen.";
        }
        else
        {
            WindowPinToggleButtonIcon.Source = CustomIcons.Unpinned(SystemColors.AccentColor);
            WindowPinToggleButton.ToolTip = "Pin the window to the screen.";
        }
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
        InputLength.Visibility = string.IsNullOrEmpty(InputTextBox.Text) ? Visibility.Collapsed : Visibility.Visible;
        SetDetectedCborDecodeSuggestionGridVisibility(Visibility.Collapsed);
        SetClipoboardSuggestionGridVisibility(Visibility.Collapsed);

        if (!_convertOnTextChange)
        {
            return;
        }
        
        _typingTimer.Stop();
        _convertingTimer.Stop();

        if (_convertImmediately || string.IsNullOrEmpty(InputTextBox.Text))
        {
            _convertImmediately = false;
            _convertingTimer.Start();
        }
        else
        {
            _typingTimer.Start();
        }
    }

    private void InputTextBoxGotFocus(object sender, RoutedEventArgs e)
    {
        _lastCachedClipboardContent = Clipboard.GetText();
        Task.Run(() =>
        {
            if (_lastCachedClipboardContent.ContainsCbor(out string detectedCborHex) && detectedCborHex != _cancelledDetectedCborInClipboard)
            {
                _detectedCborInClipboard = detectedCborHex;
            }
            else
            {
                _detectedCborInClipboard = string.Empty;
            }

            if (!string.IsNullOrEmpty(_lastCachedClipboardContent))
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
                            SetClipoboardSuggestionGridVisibility(Visibility.Visible);
                        }

                        if (!string.IsNullOrEmpty(_detectedCborInClipboard) && InputTextBox.Text != _detectedCborInClipboard)
                        {
                            SetDetectedCborDecodeSuggestionGridVisibility(Visibility.Visible);
                        }
                    }
                });
            }
        });
    }

    private void ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Task.Run(() =>
        {
            Task.Delay(20).Wait();
            Dispatcher.Invoke(() =>
            {
                if (e.Source is ComboBox inputComboBox && inputComboBox.Name == "InputComboBox")
                {
                    _manuallySelectedInputType = (ConvertingTypes)InputComboBox.SelectedItem;
                    var outputTypes = Utils.GetOutputConvertingTypes(InputTextBox.Text, (ConvertingTypes)InputComboBox.SelectedItem);
                    OutputComboBox.ItemsSource = outputTypes;
                    if (_manuallySelectedOutputType != null && outputTypes.Contains(_manuallySelectedOutputType.Value))
                    {
                        OutputComboBox.SelectedItem = _manuallySelectedOutputType;
                    }
                    else
                    {
                        OutputComboBox.SelectedIndex = 0;
                    }
                }

                if (e.Source is ComboBox outputComboBox && outputComboBox.Name == "OutputComboBox")
                {
                    _manuallySelectedOutputType = (ConvertingTypes)OutputComboBox.SelectedItem;
                }

                SetEncodingDecodingComboBoxVisibility();

                if (_convertOnTypeChange)
                {
                    Convert();
                }
            });
        });
    }

    private void PanelViewChangeButtonClick(object sender, RoutedEventArgs e)
    {
        SetupContentGrid(true);
    }

    private void ReverseButtonClick(object sender, RoutedEventArgs e)
    {
        var oldOutputType = (ConvertingTypes)OutputComboBox.SelectedItem;
        var oldInputType = (ConvertingTypes)InputComboBox.SelectedItem;
        var oldOutputContent = OutputTextBox.Text;

        _convertOnTypeChange = false;
        var newInputTypes = Utils.GetInputConvertingTypes(oldOutputContent);
        InputComboBox.ItemsSource= newInputTypes;
        InputComboBox.SelectedItem = oldOutputType;

        var newOutputTypes = Utils.GetOutputConvertingTypes(oldOutputContent, oldOutputType);
        OutputComboBox.ItemsSource = newOutputTypes;
        OutputComboBox.SelectedItem = oldInputType;
        _convertOnTypeChange = true;

        _convertOnTextChange = false;
        InputTextBox.Text = oldOutputContent;
        _convertOnTextChange = true;

        _manuallySelectedInputType = oldOutputType;
        _manuallySelectedOutputType = oldInputType;

        Convert();
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
        SetClipoboardSuggestionGridVisibility(Visibility.Collapsed);
        _cancelledClipboardSuggestionContent = _lastCachedClipboardContent;
        InputTextBox.Focus();
    }

    private void DetectedCborDecodeSuggestionButtonClick(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(_detectedCborInClipboard))
        {
            _manuallySelectedInputType = null;
            _manuallySelectedOutputType = null;
            _convertImmediately = true;
            InputTextBox.Text = _detectedCborInClipboard;
        }
    }

    private void DetectedCborDecodeSuggestionCancelButtonClick(object sender, RoutedEventArgs e)
    {
        SetDetectedCborDecodeSuggestionGridVisibility(Visibility.Collapsed);
        _cancelledDetectedCborInClipboard = _detectedCborInClipboard;
        InputTextBox.Focus();
    }

    private void AutoConvertInputTypeComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_initialAutoConvertingSelection)
        {
            _initialAutoConvertingSelection = false;
            return;
        }

        var inputType = GetAutoConvertingInputType();

        if (inputType is null)
        {
            AutoConvertOutputElementsStack.Visibility = Visibility.Collapsed;
            AutoConvertInputComboboxTitle.Visibility = Visibility.Collapsed;
            Properties.Settings.Default.AutoConvertingInput = 0;
            Properties.Settings.Default.Save();
            return;
        }

        AutoConvertInputComboboxTitle.Visibility = Visibility.Visible;

        SetupAutoConvertingOutputComboBox(inputType.Value, 0);
        SetupAutoConvertEncodingDecodingComboBoxVisibility(inputType.Value, Properties.Settings.Default.AutoConvertingEncodingDecoding);

        Properties.Settings.Default.AutoConvertingInput = AutoConvertInputTypeComboBox.SelectedIndex;
        Properties.Settings.Default.AutoConvertingOutput = AutoConvertOutputTypeComboBox.SelectedIndex;
        Properties.Settings.Default.Save();

        _lastAutoConvertedInput = string.Empty;
    }

    private void AutoConvertOutputTypeComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var inputType = GetAutoConvertingInputType();
        if (inputType is null)
        {
            return;
        }

        Properties.Settings.Default.AutoConvertingOutput = AutoConvertOutputTypeComboBox.SelectedIndex;
        Properties.Settings.Default.Save();

        SetupAutoConvertEncodingDecodingComboBoxVisibility(inputType.Value, Properties.Settings.Default.AutoConvertingEncodingDecoding);
        _lastAutoConvertedInput = string.Empty;
    }

    private void AutoConvertTextEncodingDecodingComboboxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Properties.Settings.Default.AutoConvertingEncodingDecoding = AutoConvertTextEncodingDecodingCombobox.SelectedIndex;
        Properties.Settings.Default.Save();
    }

    private void RemoveSpacesButtonClick(object sender, RoutedEventArgs e)
    {
        _convertImmediately = true;
        InputTextBox.Text = InputTextBox.Text.RemoveWhiteSpaces();
    }

    private void WindowPinToggleButtonClick(object sender, RoutedEventArgs e)
    {
        Properties.Settings.Default.TopMosetStatus = !Properties.Settings.Default.TopMosetStatus;
        Properties.Settings.Default.Save();
        SetupTopMostButtonAndValue();
    }
    #endregion
}
