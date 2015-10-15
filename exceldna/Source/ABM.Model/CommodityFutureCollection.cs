namespace ABM.Model
{
    using MathNet.Numerics.LinearAlgebra;

    using ABM.Analytics;

    public class CommodityFutureCollection : IObjectiveValueCollection
    {
        public double ObjectiveValue(Vector<double> parameters)
        {
            throw new System.NotImplementedException();
        }

        public Matrix<double> ObjectiveValueJacobian(Vector<double> parametersCurrent)
        {
            throw new System.NotImplementedException();
        }

        public Vector<double> Residual(Vector<double> parametersCurrent)
        {
            throw new System.NotImplementedException();
        }
    }
}