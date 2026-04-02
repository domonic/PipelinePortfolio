package terraform

import rego.v1

# Require encryption on RDS instances
deny contains msg if {
    resource := input.resource_changes[_]
    resource.type == "aws_db_instance"
    not resource.change.after.storage_encrypted
    msg := sprintf("RDS instance '%s' must have encryption enabled", [resource.address])
}

# Require encryption on EBS volumes
deny contains msg if {
    resource := input.resource_changes[_]
    resource.type == "aws_ebs_volume"
    not resource.change.after.encrypted
    msg := sprintf("EBS volume '%s' must be encrypted", [resource.address])
}

# Require encryption on SQS queues
deny contains msg if {
    resource := input.resource_changes[_]
    resource.type == "aws_sqs_queue"
    resource.change.after.kms_master_key_id == ""
    msg := sprintf("SQS queue '%s' should use KMS encryption", [resource.address])
}
