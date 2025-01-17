####################
# Global Variables #
####################

variable "tags" {
  description = "A mapping of tags to assign to the resource"
  type        = map(string)
  default     = null
}

variable "project_code" {
  type        = string
  description = "Defines the project where resources will be created"

}

variable "environment" {
  type        = string
  description = "Defines the environment to be created"
}