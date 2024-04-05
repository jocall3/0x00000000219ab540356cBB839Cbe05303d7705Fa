pragma solidity ^0.8.0;

contract WaterResourceManagement {
    mapping(address => uint) public waterCredits;

    event WaterDistributed(address indexed to, uint amount);

    function distributeWater(address to, uint amount) public {
        waterCredits[to] += amount;
        emit WaterDistributed(to, amount);
    }
}