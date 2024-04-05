// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

import "@openzeppelin/contracts/token/ERC20/ERC20.sol";

/**
 * @title XomeStandardToken
 * @dev Simple ERC20 Token example, where all tokens are pre-assigned to the creator.
 * Note they can later distribute these tokens as they wish using `transfer` and other
 * `ERC20` functions.
 */
contract XomeStandardToken is ERC20 {
    /**
     * @dev Constructor that gives msg.sender all of existing tokens.
     */
    constructor() ERC20("XomeStandardToken", "XST") {
        _mint(msg.sender, 10000000000000000000000000);
    }
}

/**
 * This contract is a simple example to demonstrate a smart contract that can interact with
 * other ERC20 tokens (such as DAI on the mainnet) by querying balances or facilitating transfers.
 * This is a simplified representation of the actions you might perform based on the C# test cases
 * you have.
 */
contract TokenInteractor {
    ERC20 public daiToken;

    constructor(address _daiTokenAddress) {
        daiToken = ERC20(_daiTokenAddress);
    }

    function checkDaiBalance(address _account) public view returns (uint256) {
        return daiToken.balanceOf(_account);
    }

    function transferDai(address _to, uint256 _amount) public {
        require(daiToken.transferFrom(msg.sender, _to, _amount), "Transfer failed");
    }
}