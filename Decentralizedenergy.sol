pragma solidity ^0.8.0;

contract EnergyDistribution {
    mapping(address => uint) public energyBalances;
    mapping(address => bool) public isProducer;

    event EnergyTransferred(address indexed from, address indexed to, uint amount);

    function produceEnergy(uint amount) public {
        require(isProducer[msg.sender], "Not a registered producer");
        energyBalances[msg.sender] += amount;
    }

    function transferEnergy(address to, uint amount) public {
        require(energyBalances[msg.sender] >= amount, "Insufficient energy");
        energyBalances[msg.sender] -= amount;
        energyBalances[to] += amount;
        emit EnergyTransferred(msg.sender, to, amount);
    }

    function registerProducer(address producer) public {
        isProducer[producer] = true;
    }
}