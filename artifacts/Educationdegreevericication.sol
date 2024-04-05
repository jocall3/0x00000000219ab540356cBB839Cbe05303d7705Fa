pragma solidity ^0.8.0;

contract EducationVerification {
    struct Credential {
        string name;
        string issuer;
        bool verified;
    }

    mapping(address => Credential[]) public userCredentials;

    function issueCredential(address user, string memory name, string memory issuer) public {
        userCredentials[user].push(Credential(name, issuer, true));
    }
}