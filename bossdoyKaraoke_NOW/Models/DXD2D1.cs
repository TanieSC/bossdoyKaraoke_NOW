using System;
using D3D = SharpDX.Direct3D;
using D3D11 = SharpDX.Direct3D11;
using DXGI = SharpDX.DXGI;
using D2D1 = SharpDX.Direct2D1;
using DW = SharpDX.DirectWrite;

namespace bossdoyKaraoke_NOW.Models
{
    public class DXD2D1 : IDisposable
    {

        D3D11.Device _d3d11Device;
        DXGI.Device _dxgiDevice;
        DXGI.Surface _backBuffer;
        DXGI.Surface _backBuffer2;
        D2D1.Device _d2dDevice;
       // D2D1.Brush _dcBrush;
        D2D1.Bitmap1 _targetBitmap;
        D2D1.Bitmap1 _targetBitmap2;

        D3D.FeatureLevel[] featureLevels = {
             D3D.FeatureLevel.Level_11_1,
             D3D.FeatureLevel.Level_11_0,
             D3D.FeatureLevel.Level_10_1,
             D3D.FeatureLevel.Level_10_0,
             D3D.FeatureLevel.Level_9_3,
             D3D.FeatureLevel.Level_9_2,
             D3D.FeatureLevel.Level_9_1
            };

        public DXD2D1()
        {
            _d3d11Device = new D3D11.Device(D3D.DriverType.Hardware, D3D11.DeviceCreationFlags.BgraSupport, featureLevels);
            _dxgiDevice = _d3d11Device.QueryInterface<D3D11.Device>().QueryInterface<DXGI.Device>();

            _d2dDevice = new D2D1.Device(_dxgiDevice);

          //  d2dContext = new D2D1.DeviceContext(_d2dDevice, D2D1.DeviceContextOptions.None);
           // d2dContextCdgText = new D2D1.DeviceContext(_d2dDevice, D2D1.DeviceContextOptions.None);
        }

        public void InitScreen1(IntPtr handle) {
            d2dContext = new D2D1.DeviceContext(_d2dDevice, D2D1.DeviceContextOptions.None);
            d2dContextCdgText = new D2D1.DeviceContext(_d2dDevice, D2D1.DeviceContextOptions.None);
            // DXGI SwapChain
            DXGI.SwapChainDescription swapChainDesc = new DXGI.SwapChainDescription()
            {
                BufferCount = 1,
                Usage = DXGI.Usage.RenderTargetOutput,
                OutputHandle = handle,
                IsWindowed = true,
                ModeDescription = new DXGI.ModeDescription(0, 0, new DXGI.Rational(60, 1), DXGI.Format.B8G8R8A8_UNorm),
                SampleDescription = new DXGI.SampleDescription(1, 0),
                SwapEffect = DXGI.SwapEffect.Discard
            };
            swapChain = new DXGI.SwapChain(_dxgiDevice.GetParent<DXGI.Adapter>().GetParent<DXGI.Factory>(), _d3d11Device, swapChainDesc);
            // BackBuffer
            _backBuffer = DXGI.Surface.FromSwapChain(swapChain, 0);
            //BackBuffer DeviceContext
            _targetBitmap = new D2D1.Bitmap1(d2dContext, _backBuffer);
            d2dContext.Target = _targetBitmap;
            // _dcBrush = new D2D1.SolidColorBrush(d2dContext, Color.Black);
        }

        public void InitScreen2(IntPtr handle,DeviceSettings settings)
        {
            dw_Factory = new DW.Factory();
            d2dContext2 = new D2D1.DeviceContext(_d2dDevice, D2D1.DeviceContextOptions.None);
            d2dContextCdgText2 = new D2D1.DeviceContext(_d2dDevice, D2D1.DeviceContextOptions.None);
            // DXGI SwapChain
            DXGI.SwapChainDescription swapChainDesc = new DXGI.SwapChainDescription()
            {
                BufferCount = 1,
                Usage = DXGI.Usage.RenderTargetOutput,
                OutputHandle = handle,
                IsWindowed = true,
                ModeDescription = new DXGI.ModeDescription(0, 0, new DXGI.Rational(60, 1), DXGI.Format.B8G8R8A8_UNorm),
                SampleDescription = new DXGI.SampleDescription(1, 0),
                SwapEffect = DXGI.SwapEffect.Discard
            };
            swapChain2 = new DXGI.SwapChain(_dxgiDevice.GetParent<DXGI.Adapter>().GetParent<DXGI.Factory>(), _d3d11Device, swapChainDesc);
            // BackBuffer
            _backBuffer2 = DXGI.Surface.FromSwapChain(swapChain2, 0);
            //BackBuffer DeviceContext
            _targetBitmap2 = new D2D1.Bitmap1(d2dContext2, _backBuffer2);
            d2dContext2.Target = _targetBitmap2;
            // _dcBrush = new D2D1.SolidColorBrush(d2dContext, Color.Black);
        }

        public void ResizeScreen1()
        {
            d2dContext.Target = null;
            _backBuffer.Dispose();
            _targetBitmap.Dispose();
            swapChain.ResizeBuffers(1, 0, 0, DXGI.Format.B8G8R8A8_UNorm, DXGI.SwapChainFlags.None);
            _backBuffer = DXGI.Surface.FromSwapChain(swapChain, 0);
            _targetBitmap = new D2D1.Bitmap1(d2dContext, _backBuffer);
            d2dContext.Target = _targetBitmap;
        }

        public void ResizeScreen2()
        {
            d2dContext2.Target = null;
            _backBuffer2.Dispose();
            _targetBitmap2.Dispose();
            swapChain2.ResizeBuffers(1, 0, 0, DXGI.Format.B8G8R8A8_UNorm, DXGI.SwapChainFlags.None);
            _backBuffer2 = DXGI.Surface.FromSwapChain(swapChain2, 0);
            _targetBitmap2 = new D2D1.Bitmap1(d2dContext2, _backBuffer2);
            d2dContext2.Target = _targetBitmap2;
        }

        public DXGI.SwapChain swapChain;
        public D2D1.DeviceContext d2dContext;
        public D2D1.DeviceContext d2dContextCdgText;

        public D2D1.DeviceContext d2dContext2;
        public D2D1.DeviceContext d2dContextCdgText2;

        public DXGI.SwapChain swapChain2;

        public DW.Factory dw_Factory;

        DeviceSettings settings;

        #region IDisposable Support
        /// <summary>
        /// Performs object finalization.
        /// </summary>
        ~DXD2D1()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of object resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of object resources.
        /// </summary>
        /// <param name="disposeManagedResources">If true, managed resources should be
        /// disposed of in addition to unmanaged resources.</param>
        protected virtual void Dispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                _d3d11Device.Dispose();
                _dxgiDevice.Dispose();
                _backBuffer.Dispose();
                _targetBitmap.Dispose();
                _backBuffer2.Dispose();
                _targetBitmap2.Dispose();
                _d2dDevice.Dispose();
                swapChain.Dispose();
                swapChain2.Dispose();
                d2dContext.Dispose();
                d2dContext2.Dispose();
                dw_Factory.Dispose();
            }
        }
        #endregion
    }

    public class DeviceSettings
    {
        /// <summary>
        /// Gets or sets the width of the renderable area.
        /// </summary>
        public int Width
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the height of the renderable area.
        /// </summary>
        public int Height
        {
            get;
            set;
        }

    }
}
