// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

contract DecentralizedInsurance {
    struct Policy {
        address insured;
        uint premium;
        uint coverage;
        bool active;
    }
    
    mapping(address => Policy) public policies;

    function createPolicy(uint _premium, uint _coverage) public {
        policies[msg.sender] = Policy(msg.sender, _premium, _coverage, true);
    }

    function claimInsurance() public {
        require(policies[msg.sender].active, "No active policy found");
        payable(msg.sender).transfer(policies[msg.sender].coverage);
        policies[msg.sender].active = false;
    }
}