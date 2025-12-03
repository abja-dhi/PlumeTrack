from pyEMMP.utils_xml import XMLUtils
from pyEMMP import ADCP as ADCPDataset
from pyEMMP import *
import matplotlib.pyplot as plt
import traceback
import os



def MTComparison(project, model_id, adcp_id,
                 adcp_series_mode, adcp_series_target, adcp_transect_color,
                 ssc_scale, ssc_levels, ssc_vmin, ssc_vmax, ssc_cmap_name, ssc_bottom_thresh, ssc_pixel_size_m,
                 cbar_tick_decimals, axis_tick_decimals, pad_m, distance_bin_m, bar_width_scale):
    proj_xml = XMLUtils(project)
    settings, map_settings = proj_xml.parse_settings()
    crs_helper = CRSHelper(project_crs=settings["epsg"])
    adcp_cfg = proj_xml.get_cfg_by_instrument(instrument_type="VesselMountedADCP", instrument_id=adcp_id, add_ssc=True)
    adcp = ADCPDataset(adcp_cfg, name = adcp_cfg['name'])
    mt_model_element = proj_xml.find_element(elem_id=model_id, _type="MTModel")
    mt_model_path = mt_model_element.find("Path").text
    ssc_item_number = int(mt_model_element.find("ItemNumber").text)
    mt_model = DfsuUtils2D(mt_model_path, crs_helper=crs_helper)
    if ssc_scale.lower() == "logarithmic":
        ssc_scale = "log"
    fig, axes = mt_model_transect_comparison(
        mt_model=mt_model,
        adcp=adcp,
        crs_helper=crs_helper,
        ssc_item_number=ssc_item_number,
        adcp_series_mode=adcp_series_mode,
        adcp_series_target=adcp_series_target,
        adcp_transect_color=adcp_transect_color,
        ssc_scale=ssc_scale,
        ssc_levels=ssc_levels,
        ssc_vmin=ssc_vmin,
        ssc_vmax=ssc_vmax,
        ssc_cmap_name=ssc_cmap_name,
        ssc_bottom_thresh=ssc_bottom_thresh,
        ssc_pixel_size_m=ssc_pixel_size_m,
        cbar_tick_decimals=cbar_tick_decimals,
        axis_tick_decimals=axis_tick_decimals,
        pad_m=pad_m,
        distance_bin_m=distance_bin_m,
        bar_width_scale=bar_width_scale,
        shapefile_layers=map_settings["Shapefiles"],
    )

    plt.show()



project_path = r"C:\Users\abja\AppData\Roaming\PlumeTrack\Test Nov 8.mtproj"
project = open(project_path, "r").read()
project = '\n'.join(project.splitlines()[1:])


model_id = "26"
adcp_id = "13"
adcp_series_mode = "bin"
use_mean = True
adcp_series_target = "mean"
adcp_transect_color = "black"
ssc_scale = "normal"
ssc_levels = [0.001, 0.01, 0.1, 1.0, 10.0]
ssc_vmin = None
ssc_vmax = None
ssc_cmap_name = "turbo"
ssc_bottom_thresh = 0.001
ssc_pixel_size_m = 10.0
cbar_tick_decimals = 2
axis_tick_decimals = 3
pad_m = 2000
distance_bin_m = 50
bar_width_scale = 0.15
results = MTComparison(project, model_id, adcp_id,
                       adcp_series_mode, adcp_series_target, adcp_transect_color,
                       ssc_scale, ssc_levels, ssc_vmin, ssc_vmax, ssc_cmap_name, ssc_bottom_thresh, ssc_pixel_size_m,
                       cbar_tick_decimals, axis_tick_decimals, pad_m, distance_bin_m, bar_width_scale)








r"""def PlotVelocityFlood(project, instrument_id, field_name, coord, xAxisMode, yAxisMode, cmap, vmin, vmax, title, mask):
    try:
        proj_xml = XMLUtils(project)
        cfg = proj_xml.get_cfg_by_instrument("VesselMountedADCP", instrument_id=instrument_id, add_ssc=True)
        if 'Error' in cfg.keys():
            return cfg
        adcp = ADCPDataset(cfg, name = cfg['name'])

        adcp.plot.velocity_flood_plot(
            field_name = field_name.lower(),
            coord = coord.lower(),
            y_axis_mode = yAxisMode.lower(),
            cmap = cmap,
            vmin = vmin,
            vmax = vmax,
            n_time_ticks = 6,
            title = title, 
            mask = mask,
            x_axis_mode = xAxisMode.lower()
            )
        plt.show()
        return {"Result": str(adcp._cfg)}
    except Exception as e:
        return {"Error": str(e)}



project_path = r"C:\Users\abja\AppData\Roaming\PlumeTrack\Test Nov 8.mtproj"
project = open(project_path, "r").read()
project = '\n'.join(project.splitlines()[1:])

instrument_id = "4"
field_name = "speed"
coord = "earth"
xAxisMode = "time"
yAxisMode = "depth"
cmap = "turbo"
vmin = 0
vmax = 1.75
title = None
mask = False
results = PlotVelocityFlood(project, instrument_id, field_name, coord, xAxisMode, yAxisMode, cmap, vmin, vmax, title, mask)
# print(results)

"""


r"""
def HDMTAnimation(project, hd_model_id, mt_model_id,
                                ssc_scale, ssc_levels, ssc_vmin, ssc_vmax, ssc_cmap_name, ssc_bottom_thresh,
                                model_field_pixel_size_m, model_field_quiver_stride_n, model_quiver_scale, model_quiver_width, model_quiver_headwidth, model_quiver_headlength, model_quiver_color,
                                animation_start_index, animation_end_index, animation_time_step, animation_interval, animation_output_file,
                                cbar_tick_decimals, axis_tick_decimals, bbox_path):
    try:
        proj_xml = XMLUtils(project)
        settings, map_settings = proj_xml.parse_settings()
        crs_helper = CRSHelper(project_crs=settings["epsg"])
        mt_model_element = proj_xml.find_element(elem_id=mt_model_id, _type="MTModel")
        mt_model_path = mt_model_element.find("Path").text
        ssc_item_number = int(mt_model_element.find("ItemNumber").text)
        mt_model = DfsuUtils2D(mt_model_path, crs_helper=crs_helper)
        hd_model_element = proj_xml.find_element(elem_id=hd_model_id, _type="HDModel")
        hd_model_path = hd_model_element.find("Path").text
        u_item_number = int(hd_model_element.find("UItemNumber").text)
        v_item_number = int(hd_model_element.find("VItemNumber").text)
        hd_model = DfsuUtils2D(hd_model_path, crs_helper=crs_helper)
        bbox_layer = ShapefileLayer(path=bbox_path, crs_helper=crs_helper, kind="polygon")
        if ssc_scale.lower() == "logarithmic":
            ssc_scale = "log"
        save_output = True
        if animation_output_file is None:
            save_output = False
        fig, ani = make_ssc_currents_animation(
            hd_model=hd_model,
            mt_model=mt_model,
            crs_helper=crs_helper,
            u_item_number=u_item_number,
            v_item_number=v_item_number,
            ssc_item_number=ssc_item_number,
            ssc_scale=ssc_scale,
            ssc_levels=ssc_levels,
            ssc_vmin=ssc_vmin,
            ssc_vmax=ssc_vmax,
            ssc_cmap_name=ssc_cmap_name,
            ssc_bottom_thresh=ssc_bottom_thresh,
            model_field_pixel_size_m=model_field_pixel_size_m,
            model_field_quiver_stride_n=model_field_quiver_stride_n,
            model_quiver_scale=model_quiver_scale,
            model_quiver_width=model_quiver_width,
            model_quiver_headwidth=model_quiver_headwidth,
            model_quiver_headlength=model_quiver_headlength,
            model_quiver_color=model_quiver_color,
            animation_time_start_idx=animation_start_index,
            animation_time_end_idx=animation_end_index,
            animation_time_step=animation_time_step,
            animation_interval_ms=animation_interval,
            animation_out_path=animation_output_file,
            cbar_tick_decimals=cbar_tick_decimals,
            axis_tick_decimals=axis_tick_decimals,
            shapefile_layers=map_settings["Shapefiles"],
            bbox_layer=bbox_layer,
            save_output=save_output,
            use_qt_progress=True
        )

        plt.show()
        return {"Result": "Success"}
    except Exception as e:
        return {"Error": traceback.format_exc() + "\n" + str(e)}

project_path = r"C:\Users\abja\AppData\Roaming\PlumeTrack\Test Nov 8.mtproj"
project = open(project_path, "r").read()
project = '\n'.join(project.splitlines()[1:])

hd_model_id = "24"
mt_model_id = "26"
ssc_scale = "log"
ssc_levels = [0.01,0.01,0.1,1.0,10.0]
ssc_vmin = None
ssc_vmax = None
ssc_cmap_name = "jet"
ssc_bottom_thresh = 0.001
model_field_pixel_size_m = 100.0
model_field_quiver_stride_n = 3
model_quiver_scale = 1.0
model_quiver_width = 0.005
model_quiver_headwidth = 3.0
model_quiver_headlength = 2.5
model_quiver_color = "black"
animation_start_index = 0
animation_end_index = 5
animation_time_step = 1
animation_interval = 200
animation_output_file = None
cbar_tick_decimals = 2
axis_tick_decimals = 3
bbox_path = r"C:\Users\abja\AppData\Roaming\PlumeTrack\RD7550_CEx_SG_v20250509.shp"


results = HDMTAnimation(project, hd_model_id, mt_model_id,
                                ssc_scale, ssc_levels, ssc_vmin, ssc_vmax, ssc_cmap_name, ssc_bottom_thresh,
                                model_field_pixel_size_m, model_field_quiver_stride_n, model_quiver_scale, model_quiver_width, model_quiver_headwidth, model_quiver_headlength, model_quiver_color,
                                animation_start_index, animation_end_index, animation_time_step, animation_interval, animation_output_file,
                                cbar_tick_decimals, axis_tick_decimals, bbox_path)
print(results)
"""