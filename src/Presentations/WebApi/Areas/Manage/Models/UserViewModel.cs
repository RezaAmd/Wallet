﻿namespace DigitalWallet.WebApi.Areas.Manage.Models;

public class CreateUserMVM
{
    public CreateUserMVM(Guid id)
    {
        Id = id;
    }
    public Guid Id { get; set; }
}

public class UserThumbailMVM
{
    public UserThumbailMVM(Guid id, string username, string phoneNumber, string email, string name, string surname)
    {
        Id = id;
        Username = username;
        PhoneNumber = phoneNumber;
        Email = email;
        Name = name;
        Surname = surname;
    }

    public Guid Id { get; set; }
    public string Username { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
}