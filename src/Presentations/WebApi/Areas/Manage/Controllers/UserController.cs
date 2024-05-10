﻿using DigitalWallet.Application.Dao.Identity;
using DigitalWallet.Application.Extensions;
using DigitalWallet.Application.Models;
using DigitalWallet.Application.Repositories.Wallet;
using DigitalWallet.Domain.Entities;
using DigitalWallet.Domain.Entities.Identity;
using DigitalWallet.WebApi.Areas.Manage.Models;
using Microsoft.AspNetCore.Mvc;

namespace DigitalWallet.WebApi.Areas.Manage.Controllers;

[ApiController]
[Area("Manage")]
[Route("[area]/[controller]/[action]")]
//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UserController : ControllerBase
{
    #region DI
    private readonly IUserService userService;
    private readonly IWalletRepository walletService;
    private readonly IPermissionService permissionService;
    private readonly ILogger<UserController> logger;

    public UserController(IUserService _userService,
        IWalletRepository _walletService,
        IPermissionService _permissionService,
        ILogger<UserController> _logger)
    {
        userService = _userService;
        walletService = _walletService;
        logger = _logger;
        permissionService = _permissionService;
    }
    #endregion

    [HttpGet]
    public async Task<ApiResult<object>> GetAll(string? keyword = null, int page = 1, CancellationToken cancellationToken = default)
    {
        int pageSize = 20;
        var users = await userService.GetAllAsync<UserThumbailMVM>(keyword: keyword, page: page, pageSize: pageSize, cancellationToken: cancellationToken);
        if (users.totalCount > 0)
            return Ok(users);
        return NotFound(users);
    }

    [HttpGet("{id}")]
    public async Task<ApiResult<object>> Get([FromRoute] Guid id)
    {
        var user = await userService.FindByIdAsync(id);
        if (user != null)
            return Ok(new UserThumbailMVM(user.Id, user.Username, user.PhoneNumber, user.Email, user.Name, user.Surname));
        return NotFound("کاربر مورد نظر یافت نشد.");
    }

    [HttpPost]
    [ModelStateValidator]
    //[Authorize(Roles = "CreateUser")]
    public async Task<ApiResult<object>> Create([FromBody] CreateUserMDto model, CancellationToken cancellationToken = new CancellationToken())
    {
        #region Create wallet
        // Create new wallet.
        var newWallet = new WalletEntity(model.phoneNumber);
        var createWalletResult = await walletService.CreateAsync(newWallet);
        #endregion

        #region Create user
        // Create new user.
        var newUser = new UserEntity(model.username, model.phoneNumber, model.email,
            model.name, model.surname, false, false, createWalletResult.Succeeded ? newWallet.Id : null);
        var createUserResult = await userService.CreateAsync(newUser, model.password, cancellationToken);
        if (createUserResult.Succeeded)
        {
            // If the user was created, update wallet.
            //newWallet.OwnerId = newUser.Id;
            var walletUpdateResult = await walletService.UpdateAsync(newWallet);
            if (!walletUpdateResult.Succeeded)
                logger.LogError($"Wallet {newWallet.Id} has not been updated.");
            return Ok(new CreateUserMVM(newUser.Id));
        }
        #endregion

        logger.LogError("Failed to create new user.");

        #region Delete Wallet
        // If user was not created so delete the wallet.
        var deleteWalletResult = await walletService.DeleteAsync(newWallet, cancellationToken);
        if (!deleteWalletResult.Succeeded)
            logger.LogError($"Wallet {newWallet.Id} has not been deleted.");
        #endregion

        return BadRequest(createUserResult.Errors);
    }

    [HttpPost("{id}")]
    //[Authorize(Roles = "UpdateUser")]
    public async Task<ApiResult<object>> Edit([FromRoute] Guid id, [FromBody] EditUserMDto model, CancellationToken cancellationToken)
    {
        var user = await userService.FindByIdAsync(id);
        if (user != null)
        {
            bool hasChanged = false;

            #region Validate
            if (model.username.ToLower() != user.Username.ToLower())
            {
                hasChanged = true;
                user.Username = model.username;
            }

            if (model.phoneNumber.ToLower() != user.PhoneNumber.ToLower())
            {
                hasChanged = true;
                user.PhoneNumber = model.phoneNumber;
            }

            if (model.email.ToLower() != user.Email.ToLower())
            {
                hasChanged = true;
                user.Email = model.email;
            }

            if (model.name.ToLower() != user.Name.ToLower())
            {
                hasChanged = true;
                user.Name = model.name;
            }

            if (model.surname.ToLower() != user.Surname.ToLower())
            {
                hasChanged = true;
                user.Surname = model.surname;
            }
            #endregion

            if (hasChanged)
            {
                var updateResult = await userService.UpdateAsync(user, cancellationToken);
                if (updateResult.Succeeded)
                    return Ok();
                return BadRequest(updateResult.Errors);
            }
            return BadRequest("هیچ تغییراتی صورت نگرفته است!");
        }
        return NotFound("کاربر مورد نظر پیدا نشد.");
    }

    [HttpDelete("{id}")]
    //[Authorize(Roles = "DeleteUser")]
    public async Task<ApiResult<object>> Delete([FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var user = await userService.FindByIdAsync(id);
        if (user is null)
            return NotFound("کاربر مورد نظر پیدا نشد.");

        if (user.WalletId.HasValue)
        {
            var wallet = await walletService.FindByIdAsync(user.WalletId.Value);
            if (wallet is not null)
            {
                var walletDeleteResult = await walletService.DeleteAsync(wallet, cancellationToken);
            }
        }

        var userDeleteResult = await userService.DeleteAsync(user, cancellationToken);
        if (userDeleteResult.Succeeded == false)
            return BadRequest(userDeleteResult.Errors);

        return Ok("کاربر " + user.Username + " با موفقیت حذف گردید.");
    }

    [HttpGet]
    public async Task<ApiResult<object>> AssignPermission(Guid userId, Guid permissionId,
        CancellationToken cancellationToken)
    {
        var user = await userService.FindByIdAsync(userId);
        if (user is null)
            return NotFound("کاربر مورد نظر پیدا نشد.");

        var permission = await permissionService.FindByIdAsync(permissionId, cancellationToken);
        if (permission is null)
            return NotFound("نقش مورد نظر پیدا نشد.");

        var assignResult = await userService.AddToPermissionAsync(user, permission);
        if (assignResult.Succeeded == false)
            return BadRequest(assignResult.Errors);

        return Ok($"نقش {permission.Title} با موفقیت به کاربر {user.Name} {user.Surname} اختصاص داده شد.");
    }
}