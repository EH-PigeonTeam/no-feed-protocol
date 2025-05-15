using UnityEngine.EventSystems;
using UnityEngine;

[DisallowMultipleComponent]
public class ButtonAudioToggle : ButtonAudio
{
    [Header("State Visuals")]
    [SerializeField] private GameObject m_hoverVisual;
    [SerializeField] private GameObject m_activeVisual;
    [SerializeField] private GameObject m_disabledVisual;

    [Header("Button Logic")]
    [SerializeField] private ButtonAudioToggle m_oppositeButton;

    private bool m_isActivated;

    public void SetOppositeButton(ButtonAudioToggle oppositeButton)
    {
        this.m_oppositeButton = oppositeButton;
    }

    protected override void Awake()
    {
        base.Awake();
        UpdateVisuals();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (!this.interactable || this.m_isActivated)
            return;

        SetVisual(this.m_hoverVisual, true);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        if (!this.m_isActivated)
            SetVisual(this.m_hoverVisual, false);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        if (!this.interactable)
            return;

        Activate();
    }

    /// <summary>
    /// Enters the Activate state: deactivates the others, shows the active visual, and disables the opposite.
    /// </summary>
    public void Activate()
    {
        if (this.m_isActivated)
            return;

        // Disable the opposite, if it exists
        if (this.m_oppositeButton != null)
        {
            this.m_oppositeButton.Disable();
        }

        this.m_isActivated = true;
        this.interactable = false;

        UpdateVisuals();
    }

    /// <summary>
    /// Exits the Activate state: re-enables interaction, deactivates the active visual, and re-enables the opposite if it was disabled.
    /// </summary>
    public void Deactivate()
    {
        if (!this.m_isActivated)
            return;

        this.m_isActivated = false;
        this.interactable = true;

        // Reactivates the opposite if it was disabled
        if (this.m_oppositeButton != null && !this.m_oppositeButton.interactable)
        {
            this.m_oppositeButton.Enable();
        }

        UpdateVisuals();
    }

    /// <summary>
    /// Sets the button to Disable state: it is not selectable and shows the disabled visual.
    /// </summary>
    public void Disable()
    {
        this.interactable = false;
        this.m_isActivated = false;
        UpdateVisuals();
    }

    /// <summary>
    /// Re-enables the button from the Disable state.
    /// </summary>
    public void Enable()
    {
        this.interactable = true;
        this.m_isActivated = false;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        SetVisual(this.m_hoverVisual, false);
        SetVisual(this.m_activeVisual, this.m_isActivated);
        SetVisual(this.m_disabledVisual, !this.interactable && !this.m_isActivated);
    }

    private void SetVisual(GameObject go, bool state)
    {
        if (go != null)
            go.SetActive(state);
    }

    public bool IsActivated => this.m_isActivated;
}