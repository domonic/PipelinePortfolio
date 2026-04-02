package terraform

import rego.v1

# Deny public S3 buckets
deny contains msg if {
    resource := input.resource_changes[_]
    resource.type == "aws_s3_bucket"
    resource.change.after.acl == "public-read"
    msg := sprintf("S3 bucket '%s' must not be public", [resource.address])
}

deny contains msg if {
    resource := input.resource_changes[_]
    resource.type == "aws_s3_bucket"
    resource.change.after.acl == "public-read-write"
    msg := sprintf("S3 bucket '%s' must not be public-read-write", [resource.address])
}
