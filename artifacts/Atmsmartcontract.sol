// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

contract SmartATM {
    address public bankOwner;
    mapping(address => uint) public balances;

    constructor() {
        bankOwner = msg.sender;
    }

    function deposit() public payable {
        balances[msg.sender] += msg.value;
    }

    function withdraw(uint _amount) public {
        require(balances[msg.sender] >= _amount, "Insufficient balance");
        payable(msg.sender).transfer(_amount);
        balances[msg.sender] -= _amount;
    }
}