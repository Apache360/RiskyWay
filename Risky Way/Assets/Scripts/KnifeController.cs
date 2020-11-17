﻿using System;
using UnityEngine;

public class KnifeController : MonoBehaviour
{
    public bool pause;
    public bool isRotating;
    private float _moveInput;
    public float speed;
    private int _width;
    public int lifes;
    public int crystals;
    private float distanceToCamera;
    public float invulnerabilityTime;
    public float stabbingTime;
    private float koeficientAngleCenter = 1.06000444f;
    public Material defaultMaterial;
    public Material invulnerableMaterial;
    public GameObject _knifeCenter;
    private GameObject _camera;
    private Transform _transformKnife;
    private Transform _transformCenter;
    private Transform _transformCamera;
    private Rigidbody _rigidbodyCenter;
    private CapsuleCollider _colliderCenter;
    private Vector3 _defaultCameraPosition;
    public Vector3 circleCenter;
    private Quaternion _defaultCameraRotation;
    public Quaternion _direction;
    private UIManager _UIManager;

    public void setPause(bool pause)
    {
        this.pause = pause;
    }
    void Start()
    {
        _transformKnife = GetComponent<Transform>();
        _camera = GameObject.Find("Main Camera");
        _knifeCenter = GameObject.Find("KnifeCenter"); 
        _transformCamera = _camera.GetComponent<Transform>();
        _defaultCameraPosition = _transformCamera.position;
        _defaultCameraRotation = _transformCamera.rotation;
        _rigidbodyCenter = _knifeCenter.GetComponent<Rigidbody>();
        _transformCenter = _knifeCenter.GetComponent<Transform>();
        _colliderCenter = _knifeCenter.GetComponent<CapsuleCollider>();
        _UIManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        crystals = 0;
        setStartSettings();
    }

    void Update()
    {
        float shift;
        _width = Screen.width;

        if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Other){
            if (Input.touchCount > 0){
                _moveInput = Input.GetTouch(0).position.x;
            }
        }
        if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Windows){
            _moveInput = Input.mousePosition.x;
        }

        if (!pause){
            shift = -1 * (_moveInput - (_width / 2)) * (4.8f / _width);

            _rigidbodyCenter.velocity = new Vector3(speed * (float)Math.Cos(_direction.eulerAngles.y * (Math.PI / 180)),
                    _rigidbodyCenter.velocity.y, speed * (float)Math.Sin(_direction.eulerAngles.y * (Math.PI / 180)));

            _transformKnife.rotation = Quaternion.Euler(-90, -_direction.eulerAngles.y, _transformKnife.rotation.z);

            _transformCamera.rotation = Quaternion.Euler(_defaultCameraRotation.eulerAngles.x,
                    _defaultCameraRotation.eulerAngles.y - _direction.eulerAngles.y, _defaultCameraRotation.eulerAngles.z);

            _transformCamera.position = new Vector3(_transformCenter.position.x - distanceToCamera * (float)Math.Sin((-_direction.eulerAngles.y + 75f) * (Math.PI / 180)),
                    _transformCamera.position.y, _transformCenter.position.z - distanceToCamera * (float)Math.Cos((-_direction.eulerAngles.y + 75f) * (Math.PI / 180)));

            if (shift > -2.4f && shift < 2.4f) {
                _transformKnife.position = new Vector3(_transformCenter.position.x+shift * (float)Math.Cos((_direction.eulerAngles.y+90) * (Math.PI / 180)),
                    _transformCenter.position.y, _transformCenter.position.z+shift * (float)Math.Sin((_direction.eulerAngles.y+90) * (Math.PI / 180)));
                _colliderCenter.center = new Vector3(shift * (float)Math.Cos((_direction.eulerAngles.y + 90) * (Math.PI / 180)), 
                    0, shift * (float)Math.Sin((_direction.eulerAngles.y + 90) * (Math.PI / 180)));
            }
            else{
                if (shift <= -2.4f) {
                    _transformKnife.position = new Vector3(_transformCenter.position.x + (-2.4f) * (float)Math.Cos((_direction.eulerAngles.y + 90) * (Math.PI / 180)),
                        _transformCenter.position.y, _transformCenter.position.z + (-2.4f) * (float)Math.Sin((_direction.eulerAngles.y + 90) * (Math.PI / 180)));
                    _colliderCenter.center = new Vector3(-2.4f * (float)Math.Cos((_direction.eulerAngles.y + 90) * (Math.PI / 180)),
                        0, -2.4f * (float)Math.Sin((_direction.eulerAngles.y + 90) * (Math.PI / 180)));
                }
                else {
                    _transformKnife.position = new Vector3(_transformCenter.position.x + 2.4f * (float)Math.Cos((_direction.eulerAngles.y + 90) * (Math.PI / 180)),
                        _transformCenter.position.y, _transformCenter.position.z + 2.4f * (float)Math.Sin((_direction.eulerAngles.y + 90) * (Math.PI / 180)));
                    _colliderCenter.center = new Vector3(2.4f * (float)Math.Cos((_direction.eulerAngles.y + 90) * (Math.PI / 180)),
                            0, 2.4f * (float)Math.Sin((_direction.eulerAngles.y + 90) * (Math.PI / 180)));
                }
            }                        
            if (!isRotating){
                stabilizeDirection();
            }  
        }
        else{
            _rigidbodyCenter.velocity = new Vector3(0, 0, 0);
            _transformCamera.RotateAround(_transformKnife.position, Vector3.up, 30 * Time.deltaTime);
        }

        if (lifes<1){
            _UIManager.loseScreen = true;
            pause = true;
        }
        if (invulnerabilityTime > 0){
            knifeFlickering();
        }
        else{
            GetComponent<Renderer>().material = defaultMaterial;
        }
        if (stabbingTime>0){
            knifeStabbing();
        }
    }

    public void knifeStabbing()
    {
        stabbingTime -= 8*Time.deltaTime;
        _transformKnife.position = new Vector3(_transformKnife.position.x,
            _transformKnife.position.y- stabbingTime, _transformKnife.position.z);
    }

    public void knifeFlickering()
    {
        invulnerabilityTime -= Time.deltaTime;
        GetComponent<Renderer>().material = invulnerableMaterial;
        Color tempColor = invulnerableMaterial.color;
        tempColor.a = (float)Math.Sin(30 * invulnerabilityTime);
        invulnerableMaterial.color = tempColor;
    }

    public void addCrystal()
    {
        stabbingTime = 1f;
        crystals++;
        _UIManager.updateCrystals();
    }

    public void addLife()
    {
        stabbingTime = 1f;
        lifes++;
        _UIManager.updateLifes();
    }

    public void loseLife()
    {
        if (invulnerabilityTime<=0){
            lifes--;
            if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Other) { Handheld.Vibrate(); }
            invulnerabilityTime = 1f;
            _UIManager.updateLifes();
        }
    }

    private void FixedUpdate()
    {
        if (isRotating){
            _direction = Quaternion.Euler(_direction.eulerAngles.x,
                _direction.eulerAngles.y + 90/koeficientAngleCenter * Time.deltaTime, _direction.eulerAngles.z);                        
        }
    }


    private void stabilizeDirection()
    {
        if (_direction.eulerAngles.y > -20 && _direction.eulerAngles.y < 20)
        {
            _direction = Quaternion.Euler(_direction.eulerAngles.x, 0, _direction.eulerAngles.z);
        }
        if (_direction.eulerAngles.y > 70 && _direction.eulerAngles.y < 110)
        {
            _direction = Quaternion.Euler(_direction.eulerAngles.x, 90, _direction.eulerAngles.z);
        }
        if (_direction.eulerAngles.y > 160 && _direction.eulerAngles.y < 200)
        {
            _direction = Quaternion.Euler(_direction.eulerAngles.x, 180, _direction.eulerAngles.z);
        }
        if (_direction.eulerAngles.y > 250 && _direction.eulerAngles.y < 290)
        {
            _direction = Quaternion.Euler(_direction.eulerAngles.x, 270, _direction.eulerAngles.z);
        }
    }
    public void setStartSettings()
    {      

        pause = true;
        speed = 12;
        lifes = 3;
        _direction = Quaternion.Euler(0, 0, 0);
        _transformCenter.position = new Vector3(2.5f, 5.5f, 0);
        _transformKnife.position = new Vector3(2.5f, 5.5f, 0);
        _transformCamera.position = new Vector3(_transformCenter.position.x + _defaultCameraPosition.x,
                    _transformCamera.position.y, _transformCenter.position.z + _defaultCameraPosition.z);
        _transformCamera.rotation = Quaternion.Euler(_defaultCameraRotation.eulerAngles.x,
            _defaultCameraRotation.eulerAngles.y, _defaultCameraRotation.eulerAngles.z);
        _transformKnife.rotation = Quaternion.Euler(-90, 0, _transformKnife.rotation.z);
        _UIManager.updateLifes();

        distanceToCamera = (float)Math.Sqrt(Math.Pow((_transformCamera.position.x - _transformCenter.position.x), 2)
                    + Math.Pow((_transformCamera.position.z - _transformCenter.position.z), 2));
    }
}
