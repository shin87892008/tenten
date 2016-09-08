using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Resources_Bundle
{
    private string _filename;
    UnityEngine.Object _originalObject;
    private bool _isGameObject = false;
    private Dictionary<int, ResourceItem> _resourceItems = new Dictionary<int, ResourceItem>();

    public bool IsGameObject
    {
        get { return _isGameObject; }
    }

    private void Init(string fileName, UnityEngine.Object originalObject)
    {
        if (originalObject == null)
        {
            Debug.LogError(fileName);
            return;
        }

        _filename = fileName;
        _originalObject = originalObject;
        _isGameObject = _originalObject.GetType() == typeof(GameObject);
    }

    public Resources_Bundle(string fileName, UnityEngine.Object originalObject)
    {
        Init(fileName, originalObject);
    }

    public UnityEngine.Object Create()
    {
        if (IsGameObject == false)
        {
            return Load();
        }

        foreach (var item in _resourceItems.Values)
        {
            if (item.IsUsing == false)
            {
                return item.Take();
            }
        }

        var newResourceItem = CreateResourceItem();

        return newResourceItem == null ? null : newResourceItem.Take();
    }
    private ResourceItem CreateResourceItem()
    {
        if (_originalObject == null)
        {
            Debug.LogError("Invalid file name : " + _filename);
            return null;
        }

        var instanceObject = GameObject.Instantiate(_originalObject) as GameObject;

        if (instanceObject == null)
        {
            Debug.LogError("Can't create GameObject : " + _originalObject);
            return null;
        }

        var newResourceItem = new ResourceItem(instanceObject);

        _resourceItems.Add(newResourceItem.InstanceID, newResourceItem);

        return newResourceItem;
    }
    public UnityEngine.Object Load()
    {
        return _originalObject;
    }
}


public class ResourceItem
{

    private bool _isUsing = false;
    private GameObject _gameObject = null;

    public bool IsUsing
    {
        get
        {
            return _isUsing;
        }

        private set
        {
            _isUsing = value;
            if (_isUsing == false)
            {
                _gameObject.transform.parent = Resources_Manager.Instance.transform;
            }
            _gameObject.SetActive(value);
        }
    }

    public int InstanceID
    {
        get
        {
            if (_gameObject == null)
            {
                return 0;
            }

            return _gameObject.GetInstanceID();
        }
    }

    public string Name
    {
        get
        {
            if (_gameObject == null)
            {
                return string.Empty;
            }

            return _gameObject.name;
        }
    }

    public GameObject Take()
    {
        if (IsUsing)
        {
            Debug.LogWarning("Already used Object : " + _gameObject);
            return null;
        }

        IsUsing = true;
        return _gameObject;
    }

    public void Restore()
    {
        if (IsUsing == false)
        {
            Debug.LogWarning("Don't used Object : " + _gameObject);
        }

        IsUsing = false;
    }

    public void Reset(Transform transform)
    {
        _gameObject.transform.localScale = transform.localScale;
        _gameObject.transform.localPosition = transform.localPosition;
        _gameObject.transform.localRotation = transform.localRotation;
    }

    public ResourceItem(GameObject instanceObject)
    {
        _gameObject = instanceObject;
        IsUsing = false;
    }

    private ResourceItem() { }
}
