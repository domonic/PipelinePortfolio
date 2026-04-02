package terraform

import rego.v1

required_tags := {"Project", "Environment", "ManagedBy"}

# Require mandatory tags on taggable resources
deny contains msg if {
    resource := input.resource_changes[_]
    resource.change.after.tags != null
    tags := {key | resource.change.after.tags[key]}
    missing := required_tags - tags
    count(missing) > 0
    msg := sprintf("Resource '%s' is missing required tags: %v", [resource.address, missing])
}
