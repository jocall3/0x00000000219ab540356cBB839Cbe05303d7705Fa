contract IPManagement {
    struct IP {
        string name;
        address owner;
        uint registrationDate;
    }

    mapping(uint => IP) public IPs;
    uint public ipCount;

    function registerIP(string memory _name) public {
        ipCount += 1;
        IPs[ipCount] = IP(_name, msg.sender, block.timestamp);
    }
}