pragma solidity ^0.8.0;

contract UrbanGreenSpaces {
    struct GreenSpace {
        string name;
        address[] contributors;
    }

    GreenSpace[] public greenSpaces;

    function createGreenSpace(string memory name) public {
        address[] memory contributors;
        greenSpaces.push(GreenSpace(name, contributors));
    }

    function contributeToGreenSpace(uint index) public {
        greenSpaces[index].contributors.push(msg.sender);
    }
}