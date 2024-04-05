// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

contract SupplyChain {
    struct Product {
        string name;
        address currentOwner;
        bool verified;
    }

    mapping(uint => Product) public products;

    function addProduct(uint _id, string memory _name) public {
        products[_id] = Product(_name, msg.sender, true);
    }

    function transferOwnership(uint _id, address _newOwner) public {
        require(products[_id].verified, "Product not verified");
        products[_id].currentOwner = _newOwner;
    }
}