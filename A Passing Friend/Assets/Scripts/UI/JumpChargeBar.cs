using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class JumpChargeBar : VisualElement, INotifyValueChanged<float>
{
    [SerializeField] private int _width { get; set; }
    [SerializeField] private int _height { get; set; }

    private float _m_value;

    [SerializeField] private float _minValue = 0;

    [SerializeField] private float _maxValue = 2;

    [SerializeField] private FillType _fillType;

    [SerializeField] private Color _fillColor;

    private VisualElement _cbParent;

    private VisualElement _cbBackground;

    private VisualElement _cbForeground;

    public void SetValueWithoutNotify(float newValue)
    {
        _m_value = newValue;
    }

    public float value { 
        get
        {
            _m_value = Mathf.Clamp(_m_value, _minValue, _maxValue);
            return _m_value;
        }

        set
        {
            if (EqualityComparer<float>.Default.Equals(_m_value, value))
            {
                return;  
            }

            if (panel != null)
            {
                using (ChangeEvent<float> pooled = ChangeEvent<float>.GetPooled(_m_value, value))
                {
                    pooled.target = this;
                    SetValueWithoutNotify(value);
                    SendEvent(pooled);
                }
            }
            else
            {
                SetValueWithoutNotify(value);
            }
        }
    }

    public class UmxlFactory: UxmlFactory<JumpChargeBar, UxmlTraits> { }

    public new class UxmlTraits: VisualElement.UxmlTraits
    {
        UxmlIntAttributeDescription m_width = new(){ name = "width", defaultValue = 350 };
        UxmlIntAttributeDescription m_height = new(){ name = "height", defaultValue = 100 };
        UxmlFloatAttributeDescription m_value = new(){ name = "value", defaultValue = 1 };
        UxmlEnumAttributeDescription<FillType> m_fillType = new(){ name = "fill-type", defaultValue = 0 };
        UxmlColorAttributeDescription m_fillColor = new(){ name = "fill-color", defaultValue = new Color(6, 164, 188) };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var ate = ve as JumpChargeBar;
            ate._width = m_width.GetValueFromBag(bag, cc);
            ate._height = m_height.GetValueFromBag(bag, cc);
            ate.value = m_value.GetValueFromBag(bag, cc);
            ate._fillType = m_fillType.GetValueFromBag(bag, cc);
            ate._fillColor = m_fillColor.GetValueFromBag(bag, cc);

            ate.Clear();

            VisualTreeAsset vt = Resources.Load<VisualTreeAsset>("UI/JumpChargeBar/JumpChargeBar");
            VisualElement jumpChargeBar = vt.Instantiate();
            ate._cbParent = jumpChargeBar.Q<VisualElement>("jump-charge-bar");
            ate._cbBackground = jumpChargeBar.Q<VisualElement>("background");
            ate._cbForeground = jumpChargeBar.Q<VisualElement>("foreground");
            ate.Add(jumpChargeBar);

            ate._cbParent.style.width = ate._width;
            ate._cbParent.style.height = ate._height;

            ate.style.width = ate._width;
            ate.style.height = ate._height;

            ate._cbForeground.style.backgroundColor = ate._fillColor;

            ate.RegisterValueChangedCallback(ate.UpdateChargeBar);

            ate.FillChargeBar();
        }
    }
    
    private void UpdateChargeBar(ChangeEvent<float> evt)
    {
        FillChargeBar();
    }

    private void FillChargeBar()
    {
        if (_fillType == FillType.Horizontal)
        {
            _cbForeground.style.scale = new Scale(new Vector3(value, 1, 1f));
        }
        else
        {
            _cbForeground.style.scale = new Scale(new Vector3(1, value, 1f));
        }
    }
}
