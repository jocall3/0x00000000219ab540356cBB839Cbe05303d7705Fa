// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

contract SmartCreditCard {
    address public owner;
    uint public spendingLimit;

    constructor() {
        owner = msg.sender;
        spendingLimit = 1 ether; // Example limit
    }

    receive() external payable {}

    function setSpendingLimit(uint _limit) public {
        require(msg.sender == owner, "Only the owner can set the spending limit");
        spendingLimit = _limit;
    }

    function makePayment(address payable _to, uint _amount) public {
        require(msg.sender == owner, "Only the owner can make a payment");
        require(_amount <= spendingLimit, "Amount exceeds spending limit");
        
        _to.transfer(_amount);
    }
}