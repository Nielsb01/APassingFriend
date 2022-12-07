using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class JumpChargeBar : VisualElement, INotifyValueChanged<float>
{
    [SerializeField] private int width { get; set; }
    [SerializeField] private int height { get; set; }

    public void SetValueWithoutNotify(float newValue)
    {
        m_value = newValue;
    }

    private float m_value;

    [SerializeField] private float minValue = 0;
    [SerializeField] private float maxValue = 2;
      
    public float value { 
        get
        {
            m_value = Mathf.Clamp(m_value, minValue, maxValue);
            return m_value;
        }

        set
        {
            if (EqualityComparer<float>.Default.Equals(m_value, value))
            {
                return;  
            }

            if (this.panel != null)
            {
                using (ChangeEvent<float> pooled = ChangeEvent<float>.GetPooled(this.m_value, value))
                {
                    pooled.target = (IEventHandler)this;
                    this.SetValueWithoutNotify(value);
                    this.SendEvent((EventBase)pooled);
                }
            }
            else
            {
                SetValueWithoutNotify(value);
            }
        }
    }

    [SerializeField]
    private enum FillType
    {
        Horizontal,
        Vertical
    }

    [SerializeField] private FillType fillType;


    private VisualElement hbParent;

    private VisualElement hbBackground;

    private VisualElement hbForeground;

    public new class UmxlFactory: UxmlFactory<JumpChargeBar, UxmlTraits> { }

    public new class UxmlTraits: VisualElement.UxmlTraits
    {
        UxmlIntAttributeDescription m_width = new UxmlIntAttributeDescription(){ name = "width", defaultValue = 350 };
        UxmlIntAttributeDescription m_height = new UxmlIntAttributeDescription(){ name = "height", defaultValue = 100 };
        UxmlFloatAttributeDescription m_value = new UxmlFloatAttributeDescription(){ name = "value", defaultValue = 1 };
        UxmlEnumAttributeDescription<JumpChargeBar.FillType> m_fillType = new UxmlEnumAttributeDescription<JumpChargeBar.FillType>() { name = "fill-type", defaultValue = 0 };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var ate = ve as JumpChargeBar;
            ate.width = m_width.GetValueFromBag(bag, cc);
            ate.height = m_height.GetValueFromBag(bag, cc);
            ate.value = m_value.GetValueFromBag(bag, cc);
            ate.fillType = m_fillType.GetValueFromBag(bag, cc);

            ate.Clear();

            VisualTreeAsset vt = Resources.Load<VisualTreeAsset>("UI/JumpChargeBar/JumpChargeBar");
            VisualElement jumpChargeBar = vt.Instantiate();
            ate.hbParent = jumpChargeBar.Q<VisualElement>("jump-charge-bar");
            ate.hbBackground = jumpChargeBar.Q<VisualElement>("background");
            ate.hbForeground = jumpChargeBar.Q<VisualElement>("foreground");
            ate.Add(jumpChargeBar);

            ate.hbParent.style.width = ate.width;
            ate.hbParent.style.height = ate.height;

            ate.style.width = ate.width;
            ate.style.height = ate.height;

            ate.RegisterValueChangedCallback(ate.UpdateChargeBar);

            ate.FillChargeBar();
        }
    }
    
    public void UpdateChargeBar(ChangeEvent<float> evt)
    {
        FillChargeBar();
    }

    private void FillChargeBar()
    {
        if (fillType == FillType.Horizontal)
        {
            hbForeground.style.scale = new Scale(new Vector3(value, 1, 1f));
        }
        else
        {
            hbForeground.style.scale = new Scale(new Vector3(1, value, 1f));
        }
    }
}
