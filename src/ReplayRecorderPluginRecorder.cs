using System;
using System.IO;
using TEdit.ViewModel;

namespace TEdit.Editor.Plugins;

public class ReplayRecorder
{
    private DateTime _startTime;
    private int _lastUndoIndex;

    public ReplayFile Recording { get; private set; }
    public bool IsRecording { get; private set; }

    public void Start()
    {
        _startTime = DateTime.UtcNow;
        _lastUndoIndex = ViewModelLocator.WorldViewModel.UndoManager.CurrentIndex;
        IsRecording = true;

        Recording = new ReplayFile { StartTime = _startTime };
    }

    public void Poll()
    {
        if (!IsRecording) return;

        var undoManager = ViewModelLocator.WorldViewModel.UndoManager;
        int currentIndex = undoManager.CurrentIndex;
        if (currentIndex <= _lastUndoIndex) return;

        string undoDir = Path.GetDirectoryName(undoManager.GetUndoFileName());

        for (int i = _lastUndoIndex; i < currentIndex; i++)
        {
            string undoFile = Path.Combine(undoDir, $"undo_temp_{i}");
            if (!File.Exists(undoFile)) continue;

            Recording.Frames.Add(new ReplayFrame
            {
                Index = Recording.Frames.Count,
                Time = (long)(DateTime.UtcNow - _startTime).TotalMilliseconds,
                Data = File.ReadAllBytes(undoFile),
            });
        }

        _lastUndoIndex = currentIndex;
    }

    public ReplayFile Stop()
    {
        IsRecording = false;
        Recording.TotalTime = (long)(DateTime.UtcNow - _startTime).TotalMilliseconds;
        Recording.BaselineWorld = ViewModelLocator.WorldViewModel.CurrentWorld;
        return Recording;
    }
}
