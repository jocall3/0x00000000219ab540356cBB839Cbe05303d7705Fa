// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

contract RealEstate {
    struct Property {
        address owner;
        uint price;
        bool isForSale;
    }

    mapping(uint => Property) public properties;

    function listProperty(uint _id, uint _price) public {
        properties[_id] = Property(msg.sender, _price, true);
    }

    function buyProperty(uint _id) public payable {
        require(properties[_id].isForSale, "Property not for sale");
        require(msg.value >= properties[_id].price, "Insufficient funds");
        
        payable(properties[_id].owner).transfer(msg.value);
        properties[_id].owner = msg.sender;
        properties[_id].isForSale = false;
    }
}