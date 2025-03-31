using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public Camera[] cameras; // Массив камер для переключения
    public KeyCode switchKey = KeyCode.E; // Клавиша для переключения (можно изменить в инспекторе)
    private int currentCameraIndex = 0; // Индекс текущей активной камеры

    private int direction = 1;

    void Start()
    {
        // Отключаем все камеры, кроме первой
        for (int i = 1; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(false);
        }

        // Убедимся, что есть хотя бы одна камера
        if (cameras.Length == 0)
        {
            Debug.LogError("Нет камер для переключения! Добавьте камеры в массив 'Cameras'.");
            enabled = false; // Отключаем скрипт, чтобы избежать ошибок
            return;
        }

        if (cameras[0] == null)
        {
            Debug.LogError("Первая камера не назначена!");
            enabled = false;
            return;
        }

        //Включаем первую камеру, если она выключена
        cameras[0].gameObject.SetActive(true);
    }

    void Update()
    {
        // Проверяем нажатие клавиши
        if (Input.GetKeyDown(switchKey))
        {
            SwitchCamera();
        }
    }

    void SwitchCamera()
    {
        // Деактивируем текущую камеру
        cameras[currentCameraIndex].gameObject.SetActive(false);

        // Переходим к следующей камере (с учетом направления)
        currentCameraIndex += direction;

        // Обрабатываем границы массива
        if (currentCameraIndex >= cameras.Length)
        {
            currentCameraIndex = cameras.Length - 2; // Переходим к предпоследней камере
            direction = -1; // Меняем направление на обратное
        }
        else if (currentCameraIndex < 0)
        {
            currentCameraIndex = 1; // Переходим ко второй камере
            direction = 1; // Меняем направление на прямое
        }

        // Активируем новую текущую камеру
        cameras[currentCameraIndex].gameObject.SetActive(true);
    }
}