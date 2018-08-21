using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectPers : MonoBehaviour {


    [SerializeField]
    private Image _mainRect;
    [SerializeField]
    private float _mainRectFade = 10;
    [SerializeField]
    private KeyCode _addKey = KeyCode.LeftControl;

    private float _timer;
    private float _delay = .25f;
    private Rect _rect;
    private bool _canDraw, _one_click;
    private Vector2 _startPos, _endPos;
    private Color _original, _clear, _curColor;
    private static List<UnitController> _unit;
    private UnitController _lastUnit, _currentUnit;

    public static List<UnitController> Selected { get; private set; }
    void Awake()
    {   
        _unit = new List<UnitController>();
        Selected = new List<UnitController>();
        _original = _mainRect.color;
        _clear = _original;
        _clear.a = 0;
        _curColor = _clear;
        _mainRect.color = _clear;
    }


    public static void AddUnit(UnitController comp)
    {

        _unit.Add(comp);
    }


    void Deselect()
    {
        foreach (UnitController Target in Selected)
        {
            if (Target != null)
            {
                Target.Deselect();
            }
        }
    }

    void SetSelected()
    {
        foreach (UnitController Target in _unit)
        {
            if (Target != null)
            {
                Vector2 pos = Camera.main.WorldToScreenPoint(Target.transform.position);
                pos = new Vector2(pos.x, Screen.height - pos.y);

                if (_rect.Contains(pos))
                {
                    Target.Select();
                    Selected.Add(Target);
                }
            }
        }
    }

    void Controls()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!_one_click)
            {
                // one click
                _one_click = true;
                _timer = Time.time;

                if (!HitUnit())
                {
                    _startPos = Input.mousePosition;
                    _canDraw = true;
                    _lastUnit = null;
                }
                else
                {
                    _lastUnit = _currentUnit;
                }
            }
            else
            {
                // double click
                _one_click = false;
                _canDraw = false;

                if (HitUnit() && _lastUnit != null)
                {
                    if (_lastUnit.GetInstanceID() == _currentUnit.GetInstanceID())
                    {
                        DoubleClickSelected();
                    }
                }
            }
        }


        if (_one_click)
        {
            if ((Time.time - _timer) > _delay)
            {
                _one_click = false;
            }
        }

        if (Input.GetMouseButtonUp(0) && _canDraw)
        {
            _curColor = _clear;
            _canDraw = false;
            SetSelected();
        }
    }

    void DoubleClickSelected()
    {
        foreach(UnitController Target in _unit )
        {
            if(Target!=null && !Target.UnitProperty.IsSelect)
            {
                Target.Select();
                Selected.Add(Target);
            }
        }
    }

    bool HitUnit()
    {
        if (!Input.GetKey(_addKey))
        {
            Deselect();
            Selected.Clear();
        }

        _rect = new Rect();
        _currentUnit = null;

        
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            _currentUnit = hit.collider.GetComponent<UnitController>();
        }
        
        if (_currentUnit != null)
        {
            _currentUnit.Select();
            Selected.Add(_currentUnit);
            return true;
        }

        return false;
    }

    void Draw()
    {
        _endPos = Input.mousePosition;
        if (_startPos == _endPos || !_canDraw) return;

        _curColor = _original;

        _rect = new Rect(Mathf.Min(_endPos.x, _startPos.x),
            Screen.height - Mathf.Max(_endPos.y, _startPos.y),
            Mathf.Max(_endPos.x, _startPos.x) - Mathf.Min(_endPos.x, _startPos.x),
            Mathf.Max(_endPos.y, _startPos.y) - Mathf.Min(_endPos.y, _startPos.y)
        );

        _mainRect.rectTransform.sizeDelta = new Vector2(_rect.width, _rect.height);

        _mainRect.rectTransform.position = new Vector2(_rect.x + _mainRect.rectTransform.sizeDelta.x / 2, Mathf.Max(_endPos.y, _startPos.y) - _mainRect.rectTransform.sizeDelta.y / 2);
    }

    void Update()
    {
        Controls();

        Draw();

        _mainRect.color = Color.Lerp(_mainRect.color, _curColor, _mainRectFade * Time.deltaTime);
    }
}
