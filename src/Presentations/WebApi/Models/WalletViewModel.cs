﻿using DigitalWallet.Domain.Enums;

namespace DigitalWallet.WebApi.Models;

public class WalletDetailVM
{
    public WalletDetailVM() { }
    public WalletDetailVM(Guid id, double balance = 0, string bankId = null, string createdDateTime = null)
    {
        Id = id;
        Balance = balance;
        BankId = bankId;
        CreatedDateTime = !string.IsNullOrEmpty(createdDateTime) ? createdDateTime : PersianDateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
    }
    public Guid Id { get; set; }
    public double Balance { get; set; }
    public string? BankId { get; set; } = null;
    public string CreatedDateTime { get; set; }
}

public class GetBalanceVM
{
    public GetBalanceVM(double balance)
    {
        if (balance <= 0)
        {
            Balance = 0;
        }
        else
            Balance = balance;
    }

    public double Balance { get; set; }
}

public class IncreaseResult
{
    #region Constructors
    public IncreaseResult(string identify, double amount = 0, string dateTime = null,
        TransferState state = TransferState.Failed, double? originBalance = null, double? destinationBalance = null)
    {
        Identify = identify;
        Amount = amount;
        DateTime = dateTime == null ?
            PersianDateTime.Now.ToString("dddd, dd MMMM yyyy") : dateTime;
        State = state;
        OriginBalance = originBalance;
        DestinationBalance = destinationBalance;
    }
    #endregion

    public string Identify { get; set; }
    public double Amount { get; set; }
    public string DateTime { get; set; }
    public TransferState State { get; set; }
    public double? OriginBalance { get; set; }
    public double? DestinationBalance { get; set; }
}

public class DecreaseResult
{
    #region Constructors
    public DecreaseResult(string identify, double amount = 0, string dateTime = null,
        TransferState state = TransferState.Success, double? originBalance = null, double? destinationBalance = null, string description = null)
    {
        Identify = identify;
        Amount = amount;
        DateTime = dateTime == null ?
            PersianDateTime.Now.ToString("dddd, dd MMMM yyyy") : dateTime;
        State = state;
        OriginBalance = originBalance;
        DestinationBalance = destinationBalance;
        Description = description;
    }
    #endregion

    public string Identify { get; set; }
    public double Amount { get; set; }
    public string DateTime { get; set; }
    public TransferState State { get; set; }
    public double? OriginBalance { get; set; }
    public double? DestinationBalance { get; set; }
    public string Description { get; set; }
}

public class DepositVM
{
    #region Constructors
    public DepositVM(string getwayLink)
    {
        GetwayLink = getwayLink;
    }
    #endregion

    public string GetwayLink { get; set; }
}