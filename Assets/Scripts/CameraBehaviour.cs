using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public Camera[] cameras; // ������ ����� ��� ������������
    public KeyCode switchKey = KeyCode.E; // ������� ��� ������������ (����� �������� � ����������)
    private int currentCameraIndex = 0; // ������ ������� �������� ������

    private int direction = 1;

    void Start()
    {
        // ��������� ��� ������, ����� ������
        for (int i = 1; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(false);
        }

        // ��������, ��� ���� ���� �� ���� ������
        if (cameras.Length == 0)
        {
            Debug.LogError("��� ����� ��� ������������! �������� ������ � ������ 'Cameras'.");
            enabled = false; // ��������� ������, ����� �������� ������
            return;
        }

        if (cameras[0] == null)
        {
            Debug.LogError("������ ������ �� ���������!");
            enabled = false;
            return;
        }

        //�������� ������ ������, ���� ��� ���������
        cameras[0].gameObject.SetActive(true);
    }

    void Update()
    {
        // ��������� ������� �������
        if (Input.GetKeyDown(switchKey))
        {
            SwitchCamera();
        }
    }

    void SwitchCamera()
    {
        // ������������ ������� ������
        cameras[currentCameraIndex].gameObject.SetActive(false);

        // ��������� � ��������� ������ (� ������ �����������)
        currentCameraIndex += direction;

        // ������������ ������� �������
        if (currentCameraIndex >= cameras.Length)
        {
            currentCameraIndex = cameras.Length - 2; // ��������� � ������������� ������
            direction = -1; // ������ ����������� �� ��������
        }
        else if (currentCameraIndex < 0)
        {
            currentCameraIndex = 1; // ��������� �� ������ ������
            direction = 1; // ������ ����������� �� ������
        }

        // ���������� ����� ������� ������
        cameras[currentCameraIndex].gameObject.SetActive(true);
    }
}