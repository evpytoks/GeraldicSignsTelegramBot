using System;
using System.Globalization;

/// <summary>
/// Класс коллекции геральдических знаков.
/// </summary>
public class GeraldicSigns
{
	GeraldicSign[] _signs;


	/// <summary>
	/// Конструктор пустой коллекции.
	/// </summary>
	public GeraldicSigns()
	{
		_signs = new GeraldicSign[0];
	}


	/// <summary>
	/// Конструктор нашей коллекции по другой коллеции.
	/// </summary>
	/// <param name="signs">Коллекция по которой конструируют.</param>
	public GeraldicSigns (List<GeraldicSign> signs)
	{
        // Проверка на корректность входных данных.
        if (signs == null)
        {
            throw new ArgumentException("Аргумент не может быть null.");
        }
        foreach (GeraldicSign sign in signs)
        {
            if (sign == null)
            {
                throw new ArgumentNullException("Элемент аргумента не может быть null.");
            }
        }


		// Если передали пустую коллекцию.
		if (signs.Count == 0)
		{
			_signs = new GeraldicSign[0];
			return;
		}


		// Подготовка к копированию.
		signs.Remove(signs[0]);
        _signs = new GeraldicSign[signs.Count()];
		int ind = 0;


		// Поэлементное копирование.
		foreach (GeraldicSign gs in signs)
		{
			_signs[ind] = gs;
			++ind;
		}
	}


	public int Length
	{
		get
		{
			return _signs.Length;
		}
	}


	/// <summary>
	/// Копирование информации из коллекции.
	/// </summary>
	/// <param name="signs">Копируемая коллекция.</param>
	/// <exception cref="ArgumentNullException">Аргументы или их содержимое не могут быть null.</exception>
	public void Copy(List<GeraldicSign> signs)
	{
		// Проверка на корректность входных данных.
		if (signs == null)
		{
			throw new ArgumentNullException("Аргумент не может быть null.");
		}
		foreach (GeraldicSign sign in signs)
		{
			if (sign == null)
			{
                throw new ArgumentNullException("Элемент аргумента не может быть null.");
            }
		}


		// Подготовка коллекции к копированию.
		Array.Clear(_signs);
        _signs = new GeraldicSign[signs.Count()];


		// Копирование.
        int ind = 0;
		foreach (GeraldicSign gs in signs)
		{
			_signs[ind] = gs;
			++ind;
		}
    }


	/// <summary>
	/// Получить из данной коллекции другую.
	/// </summary>
	/// <returns>Полученная коллекция.</returns>
	public List<GeraldicSign> GetList()
	{
		var otherFormat = from gs in _signs
			   select gs;
		return otherFormat.ToList();
	}


	/// <summary>
	/// Выборка элементов коллекции по заданному полю по заданному значению.
	/// </summary>
	/// <param name="chooseType">Заданное поле.</param>
	/// <param name="chooseValue">Заданное значение.</param>
	/// <exception cref="ArgumentNullException">Входные аргументы null.</exception>
	/// <exception cref="ArgumentException">Некорректный аргумент chooseType.</exception>
	public void Choose(string chooseType, string chooseValue)
	{
		// Проверка на корректность аргументов.
		if (chooseType == null || chooseValue == null)
		{
			throw new ArgumentNullException("Входные аргументы не могут быть null.");
		}


		switch (chooseType)
		{
			case "Type":
				IEnumerable<GeraldicSign> new_signs = from gs in _signs
												  where gs.Type == chooseValue
												  select gs;
				this.Copy(new_signs.ToList());
                return;
			case "RegistrationDate":
                new_signs = from gs in _signs
                            where gs.RegistrationDate == chooseValue
                            select gs;
                this.Copy(new_signs.ToList());
                return;
			default:
				throw new ArgumentException("Невозможно такое значение ChooseType.");
		}
    }


    /// <summary>
    /// Выбор элементов коллекции по заданным значениям полей
    /// certificateHolderName и registrationDate.
    /// </summary>
    /// <param name="chooseValues">Значения полей.</param>
    /// <exception cref="ArgumentException">Входные аргументы некорреткны.</exception>
    public void Choose(string[] chooseValues)
	{
        // Проверка на корректность аргументов.
        if (chooseValues == null || chooseValues.Length != 2 || chooseValues[0] == null || chooseValues[1] == null)
		{
            throw new ArgumentException("Входные аргументы некорректны.");
        }


        IEnumerable<GeraldicSign> new_signs = from gs in _signs
                                              where gs.CertificateHolderName == chooseValues[0]
											  && gs.RegistrationDate == chooseValues[1]
                                              select gs;
        this.Copy(new_signs.ToList());
    }


	/// <summary>
	/// Сортировка элементов коллекции.
	/// </summary>
	/// <param name="sortType">Тип сортировки.</param>
	/// <exception cref="ArgumentException">Некорректный аргумент sortType.</exception>
	public void Sort(int sortType)
	{
        switch(sortType)
		{
			case 1:
                IEnumerable<GeraldicSign> new_signs = from gs in _signs
                                                      orderby gs.RegistrationNumber
                                                      select gs;
                this.Copy(new_signs.ToList());
                return;
			case 2:
                new_signs = from gs in _signs
                            orderby gs.RegistrationNumber
                            select gs;
                this.Copy(new_signs.Reverse().ToList());
                return;
			default:
                throw new ArgumentException("Невозможно такое значение sortType.");
        }
    }

}

