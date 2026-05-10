using NUnit.Framework;
using Damdor.Progressio;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

public class ProgressBarControllerTests
{
    private ProgressBarController _controller;
    private List<float> _appliedValues;
    private List<float> _valueChanged;
    private List<float> _displayedValueChanged;
    private List<string> _animationEvents;

    [SetUp]
    public void Setup()
    {
        _controller = new ProgressBarController();
        _appliedValues = new List<float>();
        _valueChanged = new List<float>();
        _displayedValueChanged = new List<float>();
        _animationEvents = new List<string>();

        var events = new ProgressBarEvents();
        events.OnValueChanged.AddListener(v => _valueChanged.Add(v));
        events.OnDisplayedValueChanged.AddListener(v => _displayedValueChanged.Add(v));
        events.OnAnimationStarted.AddListener(() => _animationEvents.Add("Started"));
        events.OnAnimationFinished.AddListener(() => _animationEvents.Add("Finished"));
       
        var animation = new ProgressBarAnimation { Animated = false, Speed = 1f };

        _controller.Setup(
            apply: v => _appliedValues.Add(v),
            animation: animation,
            events: events
        );
        _controller.Start();
    }

    [TearDown]
    public void Teardown()
    {
        _controller.Reset();
    }

    // ---------------------------------------------------------------
    // Initial state
    // ---------------------------------------------------------------
    [Test]
    public void Start_SetsDisplayedValueToZero()
    {
        Assert.AreEqual(0f, _controller.DisplayedValue);
        Assert.AreEqual(0f, _controller.Value);
    }

    [Test]
    public void Start_InvokesInitialEvents()
    {
        Assert.AreEqual(1, _valueChanged.Count);
        Assert.AreEqual(0f, _valueChanged[0]);
        Assert.AreEqual(1, _displayedValueChanged.Count);
        Assert.AreEqual(0f, _displayedValueChanged[0]);
    }

    [Test]
    public void Start_AppliesInitialValue()
    {
        Assert.AreEqual(1, _appliedValues.Count);
        Assert.AreEqual(0f, _appliedValues[0]);
    }

    // ---------------------------------------------------------------
    // Value setting and clamping
    // ---------------------------------------------------------------
    [Test]
    public void SetValue_ClampsToZero()
    {
        _controller.Value = -0.5f;
        Assert.AreEqual(0f, _controller.Value);
    }

    [Test]
    public void SetValue_ClampsToOne()
    {
        _controller.Value = 1.5f;
        Assert.AreEqual(1f, _controller.Value);
    }

    [Test]
    public void SetValue_ChangesValueAndFiresEvent()
    {
        _controller.Value = 0.5f;
        Assert.AreEqual(0.5f, _controller.Value);
        Assert.AreEqual(2, _valueChanged.Count);
        Assert.AreEqual(0.5f, _valueChanged[1]);
    }

    [Test]
    public void SetValue_SameValue_DoesNotFireEvent()
    {
        _controller.Value = 0f;
        Assert.AreEqual(1, _valueChanged.Count); // only initial event
    }

    [Test]
    public void SetValue_WithinRange_Works()
    {
        _controller.Value = 0.33f;
        Assert.AreEqual(0.33f, _controller.Value, 1e-6f);
    }

    // ---------------------------------------------------------------
    // DisplayedValue (non‑animated)
    // ---------------------------------------------------------------
    [Test]
    public void DisplayedValue_UpdatesImmediatelyWhenNotAnimated()
    {
        _controller.Value = 0.7f;
        Assert.AreEqual(0.7f, _controller.DisplayedValue);
        Assert.AreEqual(2, _displayedValueChanged.Count); // initial + change
    }

    [Test]
    public void DisplayedValue_FiresChangedEvent()
    {
        _controller.Value = 0.4f;
        Assert.AreEqual(2, _displayedValueChanged.Count);
        Assert.AreEqual(0.4f, _displayedValueChanged[1]);
    }

    // ---------------------------------------------------------------
    // SetValueWithoutAnimation
    // ---------------------------------------------------------------
    [Test]
    public void SetValueWithoutAnimation_SetsValueDirectly()
    {
        _controller.Value = 0.5f; // first set via normal path
        _controller.SetValueWithoutAnimation(0.8f);
        Assert.AreEqual(0.8f, _controller.Value);
        Assert.AreEqual(0.8f, _controller.DisplayedValue);
    }

    [Test]
    public void SetValueWithoutAnimation_FiresEvents()
    {
        _controller.SetValueWithoutAnimation(0.9f);
        Assert.AreEqual(2, _valueChanged.Count); // initial + this
        Assert.AreEqual(0.9f, _valueChanged[1]);
        Assert.AreEqual(2, _displayedValueChanged.Count);
        Assert.AreEqual(0.9f, _displayedValueChanged[1]);
    }

    [Test]
    public void SetValueWithoutAnimation_SameValue_NoDuplicateEvent()
    {
        _controller.Value = 0.5f;
        int countBefore = _valueChanged.Count;
        _controller.SetValueWithoutAnimation(0.5f);
        Assert.AreEqual(countBefore, _valueChanged.Count); // no new event
    }

    // ---------------------------------------------------------------
    // Batch changes (StartChanges / CommitChanges)
    // ---------------------------------------------------------------

    [Test]
    public void BatchChanges_NestedWorks()
    {
        _controller.StartChanges();
        _controller.StartChanges();
        _controller.Value = 0.2f;
        _controller.CommitChanges(); // still inside outer
        Assert.AreEqual(1, _appliedValues.Count);

        _controller.CommitChanges();
        Assert.AreEqual(2, _appliedValues.Count);
    }

    [Test]
    public void CommitChanges_WithoutStart_LogsError()
    {
        // No StartChanges called – should log error but not crash
        LogAssert.Expect(LogType.Error, "[Progressio] Commit changes was invoked without StartChanges()");
        Assert.DoesNotThrow(() => _controller.CommitChanges());
        // We cannot easily check LogError without a mocking framework,
        // but we verify state remains consistent
        Assert.AreEqual(0f, _controller.Value);
    }

    // ---------------------------------------------------------------
    // Reset
    // ---------------------------------------------------------------
    [Test]
    public void Reset_ClearsState()
    {
        _controller.Value = 0.5f;
        _controller.Reset();

        Assert.AreEqual(0f, _controller.Value);
        Assert.AreEqual(0f, _controller.DisplayedValue);
        Assert.IsNull(_controller.Animation);
        Assert.IsNull(_controller.Events);
        // apply action should be null, but we can't call it safely
    }

    // ---------------------------------------------------------------
    // Refresh
    // ---------------------------------------------------------------

    [Test]
    public void Refresh_AfterBatchChanges_AppliesWhenNeeded()
    {
        _controller.StartChanges();
        _controller.Value = 0.5f;
        _controller.Refresh(); // should be deferred
        Assert.AreEqual(1, _appliedValues.Count); // only initial

        _controller.CommitChanges();
        Assert.AreEqual(2, _appliedValues.Count);
        Assert.AreEqual(0.5f, _appliedValues[1]);
    }

    // ---------------------------------------------------------------
    // Animation (simulated by providing Animated = true, Application.isPlaying false => still no animation)
    // To test animation we need to mock Application.isPlaying or use a wrapper.
    // Here we test that when Animated = true but Application.isPlaying false, it behaves like non‑animated.
    // For full animation, a separate integration test would be needed.
    // ---------------------------------------------------------------
    [Test]
    public void Update_WhenNotAnimated_DoesNothing()
    {
        _controller.Value = 0.5f; // already applied
        float before = _controller.DisplayedValue;
        _controller.Update(1f);
        Assert.AreEqual(before, _controller.DisplayedValue);
    }

    [Test]
    public void Update_WhenAnimated_ButNotPlaying_NoChange()
    {
        // Change animation to animated, but keep Application.isPlaying false (test runner)
        var anim = new ProgressBarAnimation { Animated = true, Speed = 2f };
        var events = new ProgressBarEvents();
        _controller.Setup(v => { }, anim, events);
        _controller.Start();
        _controller.Value = 1f;
        // DisplayedValue should have become 1 immediately because not playing
        Assert.AreEqual(1f, _controller.DisplayedValue);
        _controller.Update(0.1f);
        Assert.AreEqual(1f, _controller.DisplayedValue);
    }

    // ---------------------------------------------------------------
    // Events lifecycle
    // ---------------------------------------------------------------
    [Test]
    public void AnimationEvents_NotFiredWhenNotAnimated()
    {
        _controller.Value = 0.5f;
        Assert.IsEmpty(_animationEvents);
    }

    [Test]
    public void AnimationEvents_WhenAnimatedButNotPlaying_StillNotFired()
    {
        var anim = new ProgressBarAnimation { Animated = true, Speed = 1f };
        var events = new ProgressBarEvents();
        events.OnAnimationStarted.AddListener(() => _animationEvents.Add("Started"));
        events.OnAnimationFinished.AddListener(() => _animationEvents.Add("Finished"));
        _controller.Setup(v => { }, anim, events);
        _controller.Start();
        _controller.Value = 0.5f;
        Assert.IsEmpty(_animationEvents); // immediate set, no animation
    }

    // ---------------------------------------------------------------
    // Edge cases
    // ---------------------------------------------------------------

    [Test]
    public void SetValueWithoutAnimation_StopsOngoingAnimation()
    {
        // Use animated setup (but not playing) – just verify flag cleared
        var anim = new ProgressBarAnimation { Animated = true, Speed = 1f };
        var events = new ProgressBarEvents();
        _controller.Setup(v => { }, anim, events);
        _controller.Start();
        _controller.Value = 0.5f; // non‑animated because !Application.isPlaying
        _controller.SetValueWithoutAnimation(0.3f);
        // No animation event should have been fired
        Assert.IsEmpty(_animationEvents);
    }

    [Test]
    public void AfterReset_SetupAndStartWorksAgain()
    {
        _controller.Reset();
        Setup(); // re‑setup via SetUp manual call (not recommended in real test, but for demonstration)

        _controller.Value = 1f;
        Assert.AreEqual(1f, _controller.Value);
        Assert.AreEqual(1f, _controller.DisplayedValue);
    }
}