using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;

namespace TEdit.Editor.Plugins;

public partial class ReplayPluginRecorderView
{
    private readonly ReplayRecorder _recorder = new();
    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromMilliseconds(50) };

    public ReplayPluginRecorderView()
    {
        InitializeComponent();
        _timer.Tick += Timer_Tick;
        Closed += (_, _) => _timer.Stop();
    }

    private void RecordButton_Click(object sender, RoutedEventArgs e)
    {
        if (_recorder.IsRecording)
            StopRecording();
        else
            StartRecording();
    }

    private void StartRecording()
    {
        RecordButton.Content = "⏹";
        _recorder.Start();
        _timer.Start();
    }

    private void StopRecording()
    {
        _timer.Stop();
        _recorder.Stop();

        var worldName = Path.GetFileNameWithoutExtension(ViewModelLocator.WorldViewModel.CurrentFile);
        var startTime = _recorder.Recording.StartTime.ToString("yyyyMMddHHmm");

        var defaultName = $"{worldName}_{startTime}";
        var defaultPath = Path.Combine(Path.GetTempPath(), $"{defaultName}.TEditReplay");
        
        var dialog = new SaveFileDialog { FileName = defaultName, Filter = "Replay Files|*.TEditReplay" };
        if (dialog.ShowDialog() == true)
            _recorder.Recording.Save(dialog.FileName);
        else
            _recorder.Recording.Save(defaultPath);

        RecordButton.Content = "⏺";
        TimerText.Text = "00:00:00";
    }

    private void OpenButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog { Filter = "Replay Files|*.TEditReplay" };
        if (dialog.ShowDialog() != true) return;

        ReplayFile file;
        try
        {
            file = new ReplayFile();
            file.Load(dialog.FileName);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to open replay file.\n\n{ex.Message}",
                "Replay", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var playerView = new ReplayPluginPlayerView();
        playerView.Load(file);
        playerView.Show();
        Close();
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        _recorder.Poll();
        if (_recorder.Recording != null)
        {
            var elapsed = DateTime.UtcNow - _recorder.Recording.StartTime;
            TimerText.Text = elapsed.ToString(@"hh\:mm\:ss");
        }
    }
}
