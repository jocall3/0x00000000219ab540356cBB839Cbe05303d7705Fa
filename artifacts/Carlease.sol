contract AutonomousVehicleLease {
    struct Lease {
        address lessee;
        uint leaseDuration;
        uint leaseCost;
        bool isActive;
    }

    mapping(uint => Lease) public vehicleLeases;

    function createLease(uint _vehicleId, uint _duration, uint _cost) public {
        vehicleLeases[_vehicleId] = Lease(msg.sender, _duration, _cost, true);
    }

    function endLease(uint _vehicleId) public {
        require(vehicleLeases[_vehicleId].lessee == msg.sender, "Not the lessee");
        vehicleLeases[_vehicleId].isActive = false;
    }
}