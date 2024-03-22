using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
/// <summary>
/// Класс объекта геральдического знака.
/// </summary>
public class GeraldicSign
{
    string _name;
    string _type;
    string _picture;
    string _description;
    string _semantics;
    string _certificateHolderName;
    string _registrationDate;
    string _registrationNumber;
    string _globalId;


    /// <summary>
    /// Констуктор знака без известной о нём информации.
    /// </summary>
    public GeraldicSign()
    {
        _name = "NoInfo";
        _type = "NoInfo";
        _picture = "NoInfo";
        _description = "NoInfo";
        _semantics = "NoInfo";
        _certificateHolderName = "NoInfo";
        _registrationDate = "NoInfo";
        _registrationNumber = "NoInfo";
        _globalId = "0";
    }


    /// <summary>
    /// Конструктор с полной информацией.
    /// </summary>
    /// <param name="name">Называние знака.</param>
    /// <param name="type">Тип знака.</param>
    /// <param name="picture">Код изображения знака.</param>
    /// <param name="desciption">Описания знака.</param>
    /// <param name="semantics">История знака.</param>
    /// <param name="certificateHolderName">Наименование обладателя свидетельства.</param>
    /// <param name="registrationDate">Дата внесения знака в реестр.</param>
    /// <param name="registrationNumber">Регистрационный номер.</param>
    /// <param name="globalId">Id знака.</param>
    /// <exception cref="ArgumentNullException">Входные аргументы null.</exception>
    public GeraldicSign(string name, string type, string picture, string desciption,
        string semantics, string certificateHolderName, string registrationDate,
        string registrationNumber, string globalId)
    {
        // Проверка на корректность аргументов.
        if (name == null || type == null || picture == null || desciption == null
            || semantics == null || certificateHolderName == null 
            || registrationDate == null || registrationNumber == null)
        {
            throw new ArgumentNullException("Аргументы не могут быть null.");
        }


        _name = name;
        _type = type;
        _picture = picture;
        _description = desciption;
        _semantics = semantics;
        _certificateHolderName = certificateHolderName;
        _registrationDate = registrationDate;
        _registrationNumber = registrationNumber;
        _globalId = globalId;
    }


    /// <summary>
    /// Свойства доступа к названию знака.
    /// </summary>
    [JsonPropertyName("name")]
    [Name("Name")]
    [Index(0)]
    public string Name
    {
        get
        {
            return _name;
        }
        set
        {
            // Проверка на корректность входных данных.
            if (value == null)
            {
                throw new ArgumentNullException("Передаваемое значение не модет быть null.");
            }

            _name = value;
        }
    }


    /// <summary>
    /// Свойство доступа к типу знака.
    /// </summary>
    [JsonPropertyName("type")]
    [Name("Type")]
    [Index(1)]
    public string Type
    {
        get
        {
            return _type;
        }
        set
        {
            // Проверка на корректность входных данных.
            if (value == null)
            {
                throw new ArgumentNullException("Передаваемое значение не модет быть null.");
            }

            _type = value;
        }
    }


    /// <summary>
    /// Свойство доступа к коду изображения знака.
    /// </summary>
    [JsonPropertyName("picture")]
    [Name("Picture")]
    [Index(2)]
    public string Picture
    {
        get
        {
            return _picture;
        }
        set
        {
            // Проверка на корректность входных данных.
            if (value == null)
            {
                throw new ArgumentNullException("Передаваемое значение не модет быть null.");
            }

            _picture = value;
        }
    }


    /// <summary>
    /// Свойство доступа к описанию знака.
    /// </summary>
    [JsonPropertyName("description")]
    [Name("Description")]
    [Index(3)]
    public string Description
    {
        get
        {
            return _description;
        }
        set
        {
            // Проверка на корректность входных данных.
            if (value == null)
            {
                throw new ArgumentNullException("Передаваемое значение не модет быть null.");
            }

            _description = value;
        }
    }


    /// <summary>
    /// Свойство доступа к истории знака.
    /// </summary>
    [JsonPropertyName("semantics")]
    [Name("Semantics")]
    [Index(4)]
    public string Semantics
    {
        get
        {
            return _semantics;
        }
        set
        {
            // Проверка на корректность входных данных.
            if (value == null)
            {
                throw new ArgumentNullException("Передаваемое значение не модет быть null.");
            }

            _semantics = value;
        }
    }


    /// <summary>
    /// Свойство досутпа к наименованию лбоадателя свидетельства.
    /// </summary>
    [JsonPropertyName("certificateHolderName")]
    [Name("CertificateHolderName")]
    [Index(5)]
    public string CertificateHolderName
    {
        get
        {
            return _certificateHolderName;
        }
        set
        {
            // Проверка на корректность входных данных.
            if (value == null)
            {
                throw new ArgumentNullException("Передаваемое значение не модет быть null.");
            }

            _certificateHolderName = value;
        }
    }


    /// <summary>
    /// Свойства доступа к дате внесения в реестр.
    /// </summary>
    [JsonPropertyName("registrationDate")]
    [Name("RegistrationDate")]
    [Index(6)]
    public string RegistrationDate
    {
        get
        {
            return _registrationDate;
        }
        set
        {
            // Проверка на корректность входных данных.
            if (value == null)
            {
                throw new ArgumentNullException("Передаваемое значение не модет быть null.");
            }

            _registrationDate = value;
        }
    }


    /// <summary>
    /// Свойство доступа к номеру регистрации.
    /// </summary>
    [JsonPropertyName("registrationNumber")]
    [Name("RegistrationNumber")]
    [Index(7)]
    public string RegistrationNumber
    {
        get
        {
            return _registrationNumber;
        }
        set
        {
            // Проверка на корректность входных данных.
            if (value == null)
            {
                throw new ArgumentNullException("Передаваемое значение не модет быть null.");
            }

            _registrationNumber = value;
        }
    }


    /// <summary>
    /// Свойство доступа к id бота.
    /// </summary>
    [JsonPropertyName("globalId")]
    [Name("global_id")]
    [Index(8)]
    public string GlobalId
    {
        get
        {
            return _globalId;
        }
        set
        {
            // Проверка на корректность, передаваемого агурмента.
            if (value == null)
            {
                throw new ArgumentException("Id не может быть отрицательным.");
            }

            _globalId = value;
        }
    }
}

