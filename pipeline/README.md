# DevOps Pipeline

Like a child tale, a DevOps pipeline has 3 stages:

* Introduction: the `setup.cake` script that guess the version and
  gather all the requirements.

* Body and climax: the `build.cake` and `test.cake` scripts that converts and
  validates the developer codes against a compiler and quality assurance tests.
  `documentation.cake` is responsible to generate the web site with the
  documentation of the product.

* Ending: the `release.cake` script that put together all the outputs of the
  project and present to the final users in different ways.
