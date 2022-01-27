using System;
using UnityEngine;

/// <summary>
///  Класс, описывающий поведение фигуры.
/// </summary>
public class FigureController : MonoBehaviour
{
    // Игровой объект для вращения фигуры.
    [SerializeField] private GameObject rotator;
    // Скорость движения фигуры вниз.
    [SerializeField] private float dropSpeed = 1;
    // Ускоренное движение фигуры вниз.
    [SerializeField] private float speedMultiplier = 2;
    // Допустимое время неподвижности фигуры в секундах.
    [SerializeField] private float dropTime = 1;

    // Контроллер сцены.
    private SceneController sceneController;
    // Счетчик времени после последнего сдвига фигуры вниз в секундах.
    private float timer = 0;
    // Угол вращения фигуры.
    private float angle = 90;
    // Исходное положение фигуры.
    private Vector3 startPostition;
    // Фиксирование фигуры после падения.
    private bool isDroped = false;




    // Событие окончания падения фигуры.
    public event EventHandler FigureDroped;
    // Свойство для привязки к контроллеру сцены.
    public SceneController SceneController
    {
        set => sceneController = value;
    }

    /// <summary>
    /// Возможные направления перемещения фигуры.
    /// </summary>
    private enum Directions
    {
        left = 0,
        right = 1,
        down = 2,
    }

    private void Start()
    {
        startPostition = SceneController.spawnPosition;
        transform.position = startPostition;

    }

    private void FigureStep()
    {
        // Добавить время кадра к счетчику времени.
        // Время контролируется для равномерного движения фигуры вниз.
        timer += Time.deltaTime;
        // Если время превысело допустимое, передвинуть фигуру вниз и запустить таймер заново.
        if (timer >= dropTime)
        {
            MoveFigure(Directions.down);
            timer = 0;
        }
    }

    private void Update()
    {
        if (!isDroped)
        {
            // Сделать фигурой шаг вниз на одну клетку, если требуется.
            FigureStep();

            // Если нажата клавиша, выполнить перемещение фигуры.
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveFigure(Directions.left);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveFigure(Directions.right);
            }

            // Если нажата кнопка вниз, ускорить движение.
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveFigure(Directions.down, speedMultiplier);
            }

            // Если нажата кливаша Пробел, повернуть фигуру.
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Rotate(angle);
            }
        }
    }

    /// <summary>
    /// Метод перемещения фигуры.
    /// </summary>
    /// <param name="direction">Направление перемещения.</param>
    private void MoveFigure(Directions direction, float speedMultiplier = 1)
    {
        Vector3 movement;
        switch (direction)
        {
            case Directions.left:
                movement = transform.TransformDirection(Vector3.left);
                if (isCorrectMove(movement))
                {
                    transform.Translate(movement);
                }
                break;

            case Directions.right:
                movement = transform.TransformDirection(Vector3.right);
                if (isCorrectMove(movement))
                {
                    transform.Translate(movement);
                }
                break;

            case Directions.down:
                movement = speedMultiplier * dropSpeed * Vector3.down;
                if (isCorrectMove(movement))
                {
                    transform.Translate(movement);
                }
                break;
        }
    }

    /// <summary>
    /// Метод вращения фигуры на заданный угол.
    /// </summary>
    /// <param name="angle">Угол поворота</param>
    private void Rotate(float angle)
    {
        rotator.transform.Rotate(0, 0, angle);
        // Если выполненный поворот недопустим, отменить его.
        // Величина поступательного перемещения, необходимая методу проверки, отсутствует.
        if (!isCorrectMove(Vector3.zero))
        {
            rotator.transform.Rotate(0, 0, -angle);
        }
    }

    /// <summary>
    /// Метод проверки допустимости перемещения фигуры.
    /// </summary>
    /// <param name="movement">Величина перемещения.</param>
    /// <returns></returns>
    private bool isCorrectMove(Vector3 movement)
    {
        // Результат проверки.
        bool result = true;

        // Расстояние от центра игрового поля до крайних положений по горизонтали и вертикали.
        int width = sceneController.ColumnCount;

        // Пометить клетку как незаполненную.
        bool isFilledCell = false;
        // Выполнить перемещение, чтобы првоерить его допустимость.
        transform.position += movement;
        // Перебрать все блоки фигуры.
        foreach (Transform block in rotator.transform)
        {
            float xPosition = block.position.x;
            float yPosition = block.position.y;

            isFilledCell = sceneController.Ground.CheckCell((int)yPosition, (int)xPosition);

            // Если достигли земли или другой упавшей фигуры, пометить текущую фигуру упавшей.
            // Вызвать событие для генерации следующей фигуры.
            if (yPosition <= 0 ||
                isFilledCell)
            {
                isDroped = true;
                FigureDroped?.Invoke(this, EventArgs.Empty);
                result = false;
                break;
            }

            // Если хотя бы один блок достиг края игрвого поля, перемещение недопустимо.
            if (xPosition >= width ||
                xPosition < 0)
            {
                result = true;
                CreateBlock();
                //break;
            }
        }
        // Отменить перемещение независимо от рельтата проверки.
        transform.position -= movement;
        // Если фигура упала, отметить заполненные ячейки игрового поля.
        if (isDroped)
        {
            FillBlocks();
        }
        return result;
    }

    private void FillBlocks()
    {
        foreach (Transform block in rotator.transform)
        {
            int rowNumber = (int)block.position.y;
            int columnNumber = (int)block.position.x;
            sceneController.Ground.FillCell(rowNumber, columnNumber);
        }
    }


    private void CreateBlock()
    {/*
        Transform oldBlock = rotator.transform.GetChild(0);
        Transform block = Instantiate<Transform>(oldBlock);
        block.SetParent(rotator.transform);

        
        block.transform.localPosition += new Vector3(-sceneController.ColumnCount, 0, 0);
        int a = 5;*/

        sceneController.SpawnNewFigure(gameObject);

    }
}
